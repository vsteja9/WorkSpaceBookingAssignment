using Microsoft.EntityFrameworkCore;
using WorkSpaceBookingAssignment.Models;

namespace WorkSpaceBookingAssignment.Repository
{
    /// <summary>
    /// PostgreSQL implementation of IUserRepository using Entity Framework Core
    /// 
    /// Use Cases:
    /// - Production environments requiring data persistence
    /// - Multi-instance deployments (multiple servers share same database)
    /// - Need for ACID transactions (Atomicity, Consistency, Isolation, Durability)
    /// - Reporting and analytics on user data
    /// 
    /// Characteristics:
    /// - Data persisted to PostgreSQL database
    /// - Survives application restarts
    /// - EF Core handles connection pooling and optimization
    /// - Transaction support for data consistency
    /// 
    /// EF Core Concepts:
    /// - DbContext: Represents database session, tracks changes
    /// - DbSet: Represents table, enables LINQ queries
    /// - Change Tracking: EF remembers modifications, writes them in SaveChangesAsync()
    /// - LINQ to SQL: Translates C# queries to SQL
    /// 
    /// SOLID Principles:
    /// - Single Responsibility: Only manages PostgreSQL data access
    /// - Dependency Inversion: Depends on IUserRepository abstraction
    /// - Open/Closed: Can use this OR InMemoryUserRepository without code changes
    /// </summary>
    public class PostgresUserRepository : IUserRepository
    {
        /// <summary>
        /// Database context injected via constructor (Dependency Injection)
        /// 
        /// Why inject instead of 'new ApplicationDbContext()'?
        /// - DI container manages lifetime (scoped per request)
        /// - Configuration comes from Program.cs (connection string, logging)
        /// - Easy to mock for testing
        /// - Supports connection pooling
        /// 
        /// DbContext Lifetime (configured in Program.cs):
        /// - Scoped: New instance per HTTP request
        /// - Request starts → new context created
        /// - Request ends → context disposed, connection returned to pool
        /// 
        /// Why readonly?
        /// - Prevents accidental reassignment
        /// - Clear intent: This field is set once in constructor
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor receives DbContext from DI container
        /// 
        /// DI Container (in Program.cs):
        /// builder.Services.AddDbContext<ApplicationDbContext>(...);
        /// builder.Services.AddScoped<IUserRepository, PostgresUserRepository>();
        /// 
        /// When controller requests IUserRepository:
        /// 1. DI sees PostgresUserRepository is registered
        /// 2. PostgresUserRepository constructor needs ApplicationDbContext
        /// 3. DI creates ApplicationDbContext with configured connection
        /// 4. DI calls this constructor with context
        /// 5. DI injects repository into controller
        /// </summary>
        public PostgresUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new user in PostgreSQL database
        /// 
        /// EF Core Process:
        /// 1. _context.Users.AddAsync(user) - Tells EF to track user as "Added"
        /// 2. await _context.SaveChangesAsync() - Generates and executes SQL INSERT
        /// 
        /// Generated SQL (approximately):
        /// INSERT INTO users (id, first_name, last_name, email, phone_number, created_at)
        /// VALUES ('guid...', 'John', 'Doe', 'john@example.com', '+1234567890', '2025-11-22 10:30:00');
        /// 
        /// Error Handling:
        /// - Unique constraint violation (duplicate email) throws DbUpdateException
        /// - We catch it and throw more user-friendly InvalidOperationException
        /// 
        /// Why async?
        /// - Database operations are I/O-bound (waiting for network/disk)
        /// - async/await frees thread to handle other requests while waiting
        /// - Improves scalability: 1000 concurrent DB operations don't need 1000 threads
        /// </summary>
        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                // Tell EF Core to track this user as a new entity
                // This doesn't execute SQL yet - just marks entity as "Added"
                await _context.Users.AddAsync(user);

                // Save changes to database - this executes the INSERT command
                // If successful, returns number of rows affected (1 in this case)
                await _context.SaveChangesAsync();

                return user;
            }
            catch (DbUpdateException ex)
            {
                // DbUpdateException: Database-level errors (constraint violations, etc.)
                // Check if it's a unique constraint violation on email
                if (ex.InnerException?.Message.Contains("idx_users_email") == true)
                {
                    throw new InvalidOperationException($"User with email {user.Email} already exists", ex);
                }

                // Re-throw if it's a different database error
                throw;
            }
        }

        /// <summary>
        /// Retrieves user by ID from database
        /// 
        /// FindAsync() is EF Core method optimized for primary key lookups:
        /// - First checks EF's change tracker (in-memory cache)
        /// - If not found, executes SELECT query
        /// 
        /// Generated SQL:
        /// SELECT id, first_name, last_name, email, phone_number, created_at, updated_at
        /// FROM users
        /// WHERE id = 'guid-value-here'
        /// LIMIT 1;
        /// 
        /// Performance:
        /// - Uses primary key index → O(log n) or O(1) depending on database
        /// - Very fast even with millions of users
        /// 
        /// Why FindAsync vs FirstOrDefaultAsync?
        /// FindAsync:
        /// - Only works with primary keys
        /// - Checks cache first (faster if entity already loaded)
        /// - Optimized for single-column or composite primary keys
        /// 
        /// FirstOrDefaultAsync:
        /// - Supports any LINQ query (Where, Select, etc.)
        /// - Always queries database (no cache check)
        /// - More flexible but slightly slower for PK lookups
        /// </summary>
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            // FindAsync optimized for primary key lookups
            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Retrieves all users from database
        /// 
        /// LINQ Query Translation:
        /// C# Code: await _context.Users.ToListAsync()
        /// SQL Generated: SELECT * FROM users;
        /// 
        /// ToListAsync():
        /// - Executes query and loads all results into memory
        /// - Returns List<User> (fully materialized)
        /// - Connection is closed after query completes
        /// 
        /// ⚠️ Performance Consideration:
        /// This loads ALL users into memory - problematic with large datasets
        /// 
        /// Better for production (pagination):
        /// public async Task<List<User>> GetUsersAsync(int page, int pageSize)
        /// {
        ///     return await _context.Users
        ///         .OrderBy(u => u.CreatedAt)
        ///         .Skip((page - 1) * pageSize)
        ///         .Take(pageSize)
        ///         .ToListAsync();
        /// }
        /// 
        /// SQL with pagination:
        /// SELECT * FROM users
        /// ORDER BY created_at
        /// LIMIT 20 OFFSET 0;  -- page 1, 20 items per page
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync()
        {
            // Execute query and materialize results
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Updates existing user in database
        /// 
        /// EF Core Change Tracking:
        /// 1. FindAsync(id) - Loads user and EF starts tracking it
        /// 2. user.Update(...) - Modify properties
        /// 3. EF detects changes (compares current vs original values)
        /// 4. SaveChangesAsync() - Generates UPDATE with only changed columns
        /// 
        /// Generated SQL (only changed columns):
        /// UPDATE users
        /// SET first_name = 'NewFirst',
        ///     email = 'newemail@example.com',
        ///     updated_at = '2025-11-22 11:00:00'
        /// WHERE id = 'guid-value';
        /// 
        /// Optimistic Concurrency:
        /// - Two users load same record simultaneously
        /// - User A updates and saves
        /// - User B updates and saves
        /// - Last write wins (User B's changes overwrite User A's)
        /// 
        /// To prevent (add to User model):
        /// [Timestamp] public byte[]? RowVersion { get; set; }
        /// EF will throw DbUpdateConcurrencyException if record changed
        /// 
        /// Why no explicit _context.Users.Update()?
        /// - EF's change tracker automatically detects modifications
        /// - Update() only needed for untracked entities
        /// - Since we loaded with FindAsync, entity is already tracked
        /// </summary>
        public async Task<User?> UpdateUserAsync(Guid id, User user)
        {
            // Load existing user from database
            // This entity is now tracked by EF Core
            var existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
            {
                return null;  // User not found
            }

            try
            {
                // Update properties using model's Update method
                // EF's change tracker notices property changes
                existingUser.Update(
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber
                );

                // Save changes - generates UPDATE SQL
                await _context.SaveChangesAsync();

                return existingUser;
            }
            catch (DbUpdateException ex)
            {
                // Handle unique constraint violation on email
                if (ex.InnerException?.Message.Contains("idx_users_email") == true)
                {
                    throw new InvalidOperationException($"Email {user.Email} is already in use", ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Deletes user from database
        /// 
        /// EF Core Delete Process:
        /// 1. FindAsync(id) - Load entity
        /// 2. _context.Users.Remove(user) - Mark as "Deleted"
        /// 3. SaveChangesAsync() - Execute DELETE SQL
        /// 
        /// Generated SQL:
        /// DELETE FROM users WHERE id = 'guid-value';
        /// 
        /// Soft Delete Alternative:
        /// Instead of removing record, mark as deleted:
        /// 
        /// public bool IsDeleted { get; set; }
        /// public DateTime? DeletedAt { get; set; }
        /// 
        /// existingUser.IsDeleted = true;
        /// existingUser.DeletedAt = DateTime.UtcNow;
        /// await _context.SaveChangesAsync();
        /// 
        /// Benefits of Soft Delete:
        /// - Maintain audit trail
        /// - Can restore deleted users
        /// - Preserve referential integrity
        /// 
        /// Query Filter (in DbContext.OnModelCreating):
        /// modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        /// Now all queries automatically exclude deleted users
        /// </summary>
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return false;  // User not found
            }

            // Mark entity as deleted
            _context.Users.Remove(user);

            // Execute DELETE SQL
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Finds user by email address (case-insensitive)
        /// 
        /// LINQ to SQL Translation:
        /// C# Code: _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower())
        /// 
        /// Generated SQL:
        /// SELECT id, first_name, last_name, email, phone_number, created_at, updated_at
        /// FROM users
        /// WHERE LOWER(email) = LOWER('john@example.com')
        /// LIMIT 1;
        /// 
        /// Index Usage:
        /// - Our unique index idx_users_email helps performance
        /// - PostgreSQL can use index for case-insensitive lookups
        /// 
        /// Alternative approaches:
        /// 
        /// 1. Case-insensitive collation (in migration):
        ///    entity.Property(e => e.Email).UseCollation("case_insensitive");
        ///    Then: WHERE email = 'john@example.com' (simpler SQL)
        /// 
        /// 2. Computed column (in migration):
        ///    EmailLower as LOWER(email) with index
        ///    Then: WHERE email_lower = 'john@example.com' (faster)
        /// 
        /// 3. EF.Functions.ILike (PostgreSQL-specific):
        ///    WHERE email ILIKE 'john@example.com' (pattern matching)
        /// 
        /// FirstOrDefaultAsync():
        /// - Executes query immediately
        /// - Returns first match or null
        /// - LIMIT 1 in SQL (efficient)
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // Case-insensitive email lookup
            // ToLower() translates to LOWER() in SQL
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        /// <summary>
        /// No Dispose needed for this class
        /// 
        /// Why?
        /// - DbContext (_context) is injected, not owned by this class
        /// - DI container manages DbContext lifetime
        /// - DbContext disposed automatically at end of request scope
        /// 
        /// Disposal chain:
        /// 1. Request completes
        /// 2. Scope ends
        /// 3. DI disposes ApplicationDbContext
        /// 4. DbContext closes database connection
        /// 5. Connection returns to pool
        /// 
        /// If we created DbContext ourselves:
        /// private readonly ApplicationDbContext _context = new ApplicationDbContext();
        /// Then we'd need: public void Dispose() => _context?.Dispose();
        /// 
        /// Rule: Whoever creates disposable object is responsible for disposing it
        /// </summary>
    }
}
