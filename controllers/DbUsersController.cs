using Microsoft.AspNetCore.Mvc;
using WorkSpaceBookingAssignment.DTOs;
using WorkSpaceBookingAssignment.Services;

namespace WorkSpaceBookingAssignment.Controllers
{
    /// <summary>
    /// Controller for Database-backed User Management
    /// 
    /// Route: /api/db/users
    /// 
    /// This controller is IDENTICAL to InMemoryUsersController
    /// Only difference: Uses different dependency injection configuration
    /// 
    /// Dependency Injection Configuration (in Program.cs):
    /// - InMemoryUsersController → UserService → InMemoryUserRepository
    /// - DbUsersController → UserService → PostgresUserRepository
    /// 
    /// This demonstrates SOLID principles in action:
    /// 
    /// OPEN/CLOSED PRINCIPLE:
    /// - Same controller code works with different repositories
    /// - No changes needed to switch data sources
    /// - Can add MySQL, MongoDB, Redis repository without touching this code
    /// 
    /// LISKOV SUBSTITUTION:
    /// - PostgresUserRepository can substitute InMemoryUserRepository
    /// - Both implement same interface (IUserRepository)
    /// - Controller behavior unchanged regardless of implementation
    /// 
    /// DEPENDENCY INVERSION:
    /// - Controller depends on IUserService (abstraction)
    /// - Service depends on IUserRepository (abstraction)
    /// - Neither depends on concrete implementations
    /// - Configuration in Program.cs wires concrete classes
    /// 
    /// Why separate controllers instead of runtime selection?
    /// 
    /// CLARITY:
    /// - Explicit routes: /api/inmemory/users vs /api/db/users
    /// - Easy testing: Test each data source independently
    /// - Clear intent: Developer knows which backend they're calling
    /// 
    /// FLEXIBILITY:
    /// - Can deploy both simultaneously
    /// - Migrate data: Read from DB, write to both, verify, switch reads
    /// - Compare performance: Load test both endpoints
    /// - A/B testing: Route some users to in-memory, others to DB
    /// 
    /// Alternative approaches:
    /// 
    /// 1. Query Parameter:
    ///    GET /api/users?storage=memory
    ///    GET /api/users?storage=db
    ///    Pro: Single controller
    ///    Con: Runtime complexity, less RESTful
    /// 
    /// 2. Header-based:
    ///    GET /api/users
    ///    X-Storage-Type: memory|db
    ///    Pro: Clean URLs
    ///    Con: Hidden behavior, harder to cache
    /// 
    /// 3. Environment-based (Production approach):
    ///    GET /api/users
    ///    Dev environment → in-memory
    ///    Prod environment → PostgreSQL
    ///    Pro: Single code path
    ///    Con: Can't easily compare both
    /// 
    /// This approach (separate controllers) is best for:
    /// - Learning and demonstration
    /// - Testing and comparison
    /// - Gradual migration
    /// </summary>
    [ApiController]
    [Route("api/db/users")]  // Different route from InMemoryUsersController
    public class DbUsersController : ControllerBase
    {
        /// <summary>
        /// Same service interface as InMemoryUsersController
        /// 
        /// But this instance is configured differently in Program.cs:
        /// 
        /// For InMemoryUsersController:
        /// services.AddScoped<IUserRepository, InMemoryUserRepository>();
        /// 
        /// For DbUsersController:
        /// services.AddScoped<IUserRepository, PostgresUserRepository>();
        /// 
        /// DI Container Magic:
        /// - Two different registrations for same interface
        /// - How does DI know which one to inject?
        /// - Answer: Named/keyed services or different service types
        /// 
        /// Actually, we'll register BOTH repositories with different keys:
        /// services.AddKeyedScoped<IUserRepository, InMemoryUserRepository>("inmemory");
        /// services.AddKeyedScoped<IUserRepository, PostgresUserRepository>("postgres");
        /// 
        /// Then inject by key:
        /// public DbUsersController([FromKeyedServices("postgres")] IUserRepository repo)
        /// 
        /// But simpler approach: Register different services
        /// services.AddScoped<InMemoryUserService>();
        /// services.AddScoped<PostgresUserService>();
        /// 
        /// Or: Single UserService, but inject repository by name
        /// We'll use a factory pattern in Program.cs (see configuration there)
        /// 
        /// [FromKeyedServices("postgres")]:
        /// - Injects the service configured for PostgreSQL
        /// - Same interface, different implementation than InMemoryUsersController
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor with keyed dependency injection for PostgreSQL
        /// DI injects UserService configured with PostgresUserRepository
        /// </summary>
        public DbUsersController([FromKeyedServices("postgres")] IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Creates a new user (stored in PostgreSQL)
        /// 
        /// HTTP: POST /api/db/users
        /// 
        /// Behavior identical to InMemoryUsersController.CreateUser
        /// But data persists to PostgreSQL database
        /// 
        /// Database Operations (via PostgresUserRepository):
        /// 1. Check if email exists: SELECT * FROM users WHERE email = ?
        /// 2. Insert new user: INSERT INTO users (...) VALUES (...)
        /// 3. Return created user with database-generated values
        /// 
        /// ACID Transaction:
        /// - Atomicity: All operations succeed or all fail
        /// - Consistency: Database constraints enforced
        /// - Isolation: Concurrent requests don't interfere
        /// - Durability: Data survives server restart
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var user = await _userService.CreateUserAsync(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber
                );

                var response = UserResponse.FromUser(user);

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = user.Id },
                    response
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a user by ID (from PostgreSQL)
        /// 
        /// HTTP: GET /api/db/users/{id}
        /// 
        /// Database Query:
        /// SELECT * FROM users WHERE id = '{id}' LIMIT 1;
        /// 
        /// Performance:
        /// - Uses primary key index (very fast)
        /// - O(log n) lookup even with millions of records
        /// - Typically < 1ms for local database
        /// - Network latency dominates for remote database
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new ErrorResponse("User not found"));
            }

            var response = UserResponse.FromUser(user);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves all users (from PostgreSQL)
        /// 
        /// HTTP: GET /api/db/users
        /// 
        /// Database Query:
        /// SELECT * FROM users;
        /// 
        /// Performance Considerations:
        /// - Loads all users from database into memory
        /// - Fine for small datasets (< 1000 users)
        /// - Problematic for large datasets (> 10,000 users)
        /// 
        /// Production recommendation:
        /// Implement pagination:
        /// GET /api/db/users?page=1&pageSize=20
        /// 
        /// Query becomes:
        /// SELECT * FROM users
        /// ORDER BY created_at DESC
        /// LIMIT 20 OFFSET 0;
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var response = users.Select(UserResponse.FromUser).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Updates an existing user (in PostgreSQL)
        /// 
        /// HTTP: PUT /api/db/users/{id}
        /// 
        /// Database Operations:
        /// 1. SELECT user WHERE id = {id}
        /// 2. If email changed, SELECT user WHERE email = {new_email}
        /// 3. UPDATE users SET ... WHERE id = {id}
        /// 
        /// Transaction handling:
        /// EF Core wraps SaveChangesAsync in transaction automatically
        /// If any operation fails, all changes rolled back
        /// 
        /// Concurrency:
        /// Current implementation: Last write wins
        /// For optimistic concurrency, add [Timestamp] to User model
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(
            Guid id,
            [FromBody] UpdateUserRequest request)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(
                    id,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber
                );

                if (user == null)
                {
                    return NotFound(new ErrorResponse("User not found"));
                }

                var response = UserResponse.FromUser(user);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Deletes a user (from PostgreSQL)
        /// 
        /// HTTP: DELETE /api/db/users/{id}
        /// 
        /// Database Operations:
        /// 1. SELECT user WHERE id = {id}
        /// 2. DELETE FROM users WHERE id = {id}
        /// 
        /// IMPORTANT: Hard delete (permanent removal)
        /// 
        /// For production, consider:
        /// - Soft delete (mark as deleted, don't remove)
        /// - Cascade delete (delete related records)
        /// - Archive (move to archive table)
        /// - Audit log (record who deleted and when)
        /// 
        /// Referential Integrity:
        /// If user has foreign key relationships:
        /// - Orders, Comments, etc.
        /// - Database will enforce constraints
        /// - May need CASCADE or SET NULL
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                return NotFound(new ErrorResponse("User not found"));
            }

            return NoContent();
        }
    }
}
