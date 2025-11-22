using WorkSpaceBookingAssignment.Models;

namespace WorkSpaceBookingAssignment.Repository
{
    /// <summary>
    /// Repository interface - defines contract for user data access
    /// 
    /// SOLID Principles Demonstrated:
    /// 
    /// 1. SINGLE RESPONSIBILITY:
    ///    - Only responsible for data access operations
    ///    - No business logic (that's in Service layer)
    ///    - No HTTP concerns (that's in Controller)
    /// 
    /// 2. OPEN/CLOSED PRINCIPLE:
    ///    - Open for extension: Can create new implementations (SQL, NoSQL, Cache)
    ///    - Closed for modification: Implementations don't change the interface
    /// 
    /// 3. LISKOV SUBSTITUTION:
    ///    - Any implementation can replace another without breaking code
    ///    - InMemoryUserRepository and PostgresUserRepository are interchangeable
    /// 
    /// 4. INTERFACE SEGREGATION:
    ///    - Focused interface: Only user-related operations
    ///    - No unnecessary methods that some implementations wouldn't need
    /// 
    /// 5. DEPENDENCY INVERSION:
    ///    - High-level modules (Services) depend on this abstraction
    ///    - Low-level modules (Repositories) implement this abstraction
    ///    - Neither depends on the other directly
    /// 
    /// Benefits:
    /// - Easy testing: Mock this interface for unit tests
    /// - Flexibility: Swap data sources without changing business logic
    /// - Clear contract: Methods are self-documenting
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Creates a new user in the data store
        /// 
        /// Async/await: All methods are async for scalability
        /// - In-memory: Still async for consistent interface
        /// - Database: Truly async, doesn't block threads while waiting for DB
        /// 
        /// Returns: The created user with generated ID and timestamps
        /// Throws: Exception if email already exists (unique constraint)
        /// </summary>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Retrieves a user by their unique identifier
        /// 
        /// Returns: User if found, null if not found
        /// Why nullable? Tells caller to handle "not found" case
        /// 
        /// Example usage:
        /// var user = await repository.GetUserByIdAsync(id);
        /// if (user == null) return NotFound();
        /// </summary>
        Task<User?> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all users from the data store
        /// 
        /// Returns: List of all users (empty list if no users)
        /// Why List<User> not IEnumerable<User>?
        /// - Materialized data: Query is executed, not deferred
        /// - Safe to use: No risk of multiple enumeration or connection issues
        /// 
        /// For large datasets, consider pagination:
        /// Task<List<User>> GetUsersAsync(int page, int pageSize);
        /// </summary>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Updates an existing user's information
        /// 
        /// Parameters:
        /// - id: User to update
        /// - user: New values for the user
        /// 
        /// Returns: Updated user if found, null if not found
        /// 
        /// Implementation notes:
        /// - In-memory: Find user in list and update properties
        /// - Database: EF Core tracks changes, SaveChangesAsync() writes to DB
        /// </summary>
        Task<User?> UpdateUserAsync(Guid id, User user);

        /// <summary>
        /// Deletes a user by their unique identifier
        /// 
        /// Returns: true if deleted, false if user not found
        /// 
        /// Why boolean return?
        /// - Clearly indicates success/failure
        /// - Alternative: void/Task and throw exception if not found
        /// </summary>
        Task<bool> DeleteUserAsync(Guid id);

        /// <summary>
        /// Finds a user by their email address
        /// 
        /// Why separate method instead of using GetAllUsersAsync().FirstOrDefault()?
        /// - Performance: Uses database index on email column
        /// - Intent: Makes code more readable
        /// - Flexibility: Can optimize query (e.g., case-insensitive search)
        /// 
        /// Used for: Duplicate email validation during user creation
        /// </summary>
        Task<User?> GetUserByEmailAsync(string email);
    }
}
