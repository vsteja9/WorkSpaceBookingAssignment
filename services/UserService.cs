using WorkSpaceBookingAssignment.Models;
using WorkSpaceBookingAssignment.Repository;

namespace WorkSpaceBookingAssignment.Services
{
    /// <summary>
    /// User service - implements business logic for user operations
    /// 
    /// This class demonstrates SOLID principles:
    /// 
    /// 1. SINGLE RESPONSIBILITY:
    ///    Only responsible for user business logic
    ///    Data access delegated to repository
    ///    HTTP concerns delegated to controller
    /// 
    /// 2. OPEN/CLOSED:
    ///    Open for extension: Can add new business methods
    ///    Closed for modification: Existing methods stable
    /// 
    /// 3. LISKOV SUBSTITUTION:
    ///    Can substitute any IUserRepository implementation
    ///    Works with both InMemoryUserRepository and PostgresUserRepository
    /// 
    /// 4. INTERFACE SEGREGATION:
    ///    Depends only on IUserRepository interface
    ///    Not coupled to concrete repository implementations
    /// 
    /// 5. DEPENDENCY INVERSION:
    ///    Depends on IUserRepository abstraction
    ///    Not on concrete PostgresUserRepository or InMemoryUserRepository
    ///    Repository injected via constructor
    /// 
    /// Business Logic Examples in this Service:
    /// - Email uniqueness validation
    /// - Input validation and sanitization
    /// - Error handling with meaningful messages
    /// - Future: Authorization, audit logging, notifications
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// Repository dependency injected via constructor
        /// 
        /// Dependency Injection Benefits:
        /// 
        /// 1. TESTABILITY:
        ///    Can mock IUserRepository for unit tests
        ///    Test business logic without database
        ///    
        ///    Example test:
        ///    var mockRepo = new Mock<IUserRepository>();
        ///    mockRepo.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync(existingUser);
        ///    var service = new UserService(mockRepo.Object);
        ///    // Test that service throws exception for duplicate email
        /// 
        /// 2. FLEXIBILITY:
        ///    Switch between in-memory and PostgreSQL without changing this code
        ///    Configure in Program.cs:
        ///    services.AddScoped<IUserRepository, InMemoryUserRepository>(); // Dev
        ///    services.AddScoped<IUserRepository, PostgresUserRepository>(); // Prod
        /// 
        /// 3. LOOSE COUPLING:
        ///    Service doesn't know about database, DbContext, or in-memory list
        ///    Only knows the contract defined by IUserRepository
        ///    Can add new repository implementations without changing service
        /// 
        /// Why readonly?
        /// - Prevents accidental reassignment
        /// - Clear intent: Set once in constructor, never changed
        /// - Thread-safe: No modifications after construction
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Constructor with dependency injection
        /// 
        /// DI Container Flow:
        /// 1. Controller constructor needs IUserService
        /// 2. DI sees UserService implements IUserService
        /// 3. UserService constructor needs IUserRepository
        /// 4. DI provides configured repository (InMemory or Postgres)
        /// 5. DI creates UserService with repository
        /// 6. DI injects UserService into controller
        /// 
        /// Configuration (Program.cs):
        /// builder.Services.AddScoped<IUserRepository, PostgresUserRepository>();
        /// builder.Services.AddScoped<IUserService, UserService>();
        /// </summary>
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Creates a new user with business validation
        /// 
        /// Business Logic Flow:
        /// 1. Validate input (basic validation)
        /// 2. Check business rules (email uniqueness)
        /// 3. Create domain model (User)
        /// 4. Persist to data store (repository)
        /// 5. Return created entity
        /// 
        /// Why validate here instead of repository?
        /// - Business rule: "Email must be unique" is business logic, not data access
        /// - Repository is dumb storage: "Save what I give you"
        /// - Service is smart logic: "Only save if business rules satisfied"
        /// 
        /// Error Handling Philosophy:
        /// - ArgumentException: Developer mistake (null/empty required fields)
        /// - InvalidOperationException: Business rule violation (duplicate email)
        /// - This guides API responses (400 vs 409)
        /// </summary>
        public async Task<User> CreateUserAsync(
            string firstName,
            string lastName,
            string email,
            string? phoneNumber)
        {
            // Input validation - catch developer errors
            // These are "should never happen" if client validates properly
            // But we check server-side for security

            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("First name is required", nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Last name is required", nameof(lastName));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required", nameof(email));
            }

            // Business rule validation - enforce business constraints
            // Check if email already exists (case-insensitive)
            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                // Business rule violated: Duplicate email
                // HTTP 409 Conflict is appropriate response
                throw new InvalidOperationException($"User with email '{email}' already exists");
            }

            // Create domain model
            // User constructor handles:
            // - ID generation (Guid.NewGuid())
            // - Timestamp creation (DateTime.UtcNow)
            // - Property initialization
            var user = new User(
                firstName.Trim(),      // Trim whitespace for consistency
                lastName.Trim(),
                email.Trim().ToLower(), // Normalize email to lowercase
                phoneNumber?.Trim()     // Handle optional phone number
            );

            // Persist to data store through repository
            // Repository handles actual storage (in-memory, PostgreSQL, etc.)
            return await _userRepository.CreateUserAsync(user);
        }

        /// <summary>
        /// Retrieves user by unique identifier
        /// 
        /// Why in service layer if it's just pass-through?
        /// 
        /// Current: Simple delegation to repository
        /// Future enhancements without changing controller:
        /// 
        /// 1. CACHING:
        ///    var cacheKey = $"user:{id}";
        ///    if (_cache.TryGetValue(cacheKey, out User? user)) return user;
        ///    user = await _userRepository.GetUserByIdAsync(id);
        ///    _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));
        ///    return user;
        /// 
        /// 2. AUTHORIZATION:
        ///    var user = await _userRepository.GetUserByIdAsync(id);
        ///    if (user != null && !_currentUser.CanView(user))
        ///        throw new UnauthorizedAccessException();
        ///    return user;
        /// 
        /// 3. AUDIT LOGGING:
        ///    var user = await _userRepository.GetUserByIdAsync(id);
        ///    if (user != null)
        ///        await _auditLog.LogAccessAsync(_currentUser.Id, user.Id, "View");
        ///    return user;
        /// 
        /// 4. SOFT DELETE FILTERING:
        ///    var user = await _userRepository.GetUserByIdAsync(id);
        ///    return user?.IsDeleted == true ? null : user;
        /// 
        /// Separation of concerns: Repository gets data, Service applies business logic
        /// </summary>
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            // Currently simple pass-through, but provides abstraction point
            // for future business logic without changing callers
            return await _userRepository.GetUserByIdAsync(id);
        }

        /// <summary>
        /// Retrieves all users from system
        /// 
        /// Production Considerations:
        /// 
        /// Current implementation returns ALL users - problematic for large datasets:
        /// - Memory usage: Loading 1 million users into memory
        /// - Performance: Slow query and serialization
        /// - Network: Large JSON response
        /// 
        /// Better approach - Pagination:
        /// public async Task<PagedResult<User>> GetUsersAsync(int page, int pageSize)
        /// {
        ///     var totalCount = await _userRepository.CountUsersAsync();
        ///     var users = await _userRepository.GetUsersPagedAsync(page, pageSize);
        ///     
        ///     return new PagedResult<User>
        ///     {
        ///         Data = users,
        ///         Page = page,
        ///         PageSize = pageSize,
        ///         TotalCount = totalCount,
        ///         TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        ///     };
        /// }
        /// 
        /// Additional enhancements:
        /// - Filtering: GetUsersAsync(string? searchTerm)
        /// - Sorting: GetUsersAsync(string sortBy, bool descending)
        /// - Field selection: GetUsersAsync(string[] fields) - return only requested fields
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        /// <summary>
        /// Updates existing user with business validation
        /// 
        /// Business Logic Flow:
        /// 1. Validate input (required fields)
        /// 2. Check if email is being changed
        /// 3. If changed, verify new email is unique
        /// 4. Update user through repository
        /// 5. Return updated user
        /// 
        /// Why validation in service, not repository?
        /// - Email uniqueness is a BUSINESS RULE
        /// - Repository is data access: "Update if you can"
        /// - Service is business logic: "Update if rules allow"
        /// 
        /// Edge Cases Handled:
        /// 1. User not found → return null (let controller decide 404 response)
        /// 2. Email unchanged → skip uniqueness check (allow keeping same email)
        /// 3. Email changed to existing → throw exception (409 Conflict)
        /// 
        /// Transaction Considerations:
        /// Current: Two repository calls (GetUserByEmail, UpdateUser)
        /// Problem: Race condition possible
        /// 
        /// Thread 1: Check email 'test@example.com' → not exists
        /// Thread 2: Check email 'test@example.com' → not exists
        /// Thread 1: Update user to 'test@example.com' → success
        /// Thread 2: Update user to 'test@example.com' → success (duplicate!)
        /// 
        /// Solution: Database unique constraint catches this
        /// Repository will throw exception on constraint violation
        /// </summary>
        public async Task<User?> UpdateUserAsync(
            Guid id,
            string firstName,
            string lastName,
            string email,
            string? phoneNumber)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("First name is required", nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Last name is required", nameof(lastName));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required", nameof(email));
            }

            // Create updated user object
            // Repository will handle finding and updating the existing user
            var updatedUser = new User(
                firstName.Trim(),
                lastName.Trim(),
                email.Trim().ToLower(),
                phoneNumber?.Trim()
            );

            // Repository handles:
            // 1. Finding user by ID
            // 2. Checking email uniqueness (if changed)
            // 3. Updating properties
            // 4. Saving changes
            // 5. Returning updated user or null if not found
            return await _userRepository.UpdateUserAsync(id, updatedUser);
        }

        /// <summary>
        /// Deletes user from system
        /// 
        /// Current Implementation: Hard delete (permanent removal)
        /// 
        /// Production Considerations:
        /// 
        /// HARD DELETE (current):
        /// - Permanently removes user from database
        /// - Cannot be undone
        /// - Breaks referential integrity if user has related data
        /// - Good for: GDPR compliance, test data cleanup
        /// 
        /// SOFT DELETE (recommended for production):
        /// public async Task<bool> SoftDeleteUserAsync(Guid id)
        /// {
        ///     var user = await _userRepository.GetUserByIdAsync(id);
        ///     if (user == null) return false;
        ///     
        ///     user.IsDeleted = true;
        ///     user.DeletedAt = DateTime.UtcNow;
        ///     await _userRepository.UpdateUserAsync(user.Id, user);
        ///     return true;
        /// }
        /// 
        /// Benefits of soft delete:
        /// - Maintain audit trail
        /// - Can restore if deleted by mistake
        /// - Preserve referential integrity (orders, etc.)
        /// - Comply with retention policies
        /// 
        /// Business Rules to Consider:
        /// - Can only delete inactive users
        /// - Cannot delete user with pending orders
        /// - Admin permission required
        /// - Log deletion for audit
        /// - Notify user via email
        /// 
        /// Example with business rules:
        /// public async Task<bool> DeleteUserAsync(Guid id, Guid deletedByUserId)
        /// {
        ///     var user = await _userRepository.GetUserByIdAsync(id);
        ///     if (user == null) return false;
        ///     
        ///     // Business rule: Cannot delete yourself
        ///     if (user.Id == deletedByUserId)
        ///         throw new InvalidOperationException("Cannot delete your own account");
        ///     
        ///     // Business rule: Check for active orders
        ///     var hasActiveOrders = await _orderRepository.HasActiveOrdersAsync(id);
        ///     if (hasActiveOrders)
        ///         throw new InvalidOperationException("Cannot delete user with active orders");
        ///     
        ///     // Delete and log
        ///     var result = await _userRepository.DeleteUserAsync(id);
        ///     if (result)
        ///         await _auditLog.LogDeletionAsync(deletedByUserId, id);
        ///     
        ///     return result;
        /// }
        /// </summary>
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            // Simple delegation to repository
            // Repository handles finding and removing user
            // Returns true if deleted, false if user not found
            return await _userRepository.DeleteUserAsync(id);
        }
    }
}
