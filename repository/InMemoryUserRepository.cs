using WorkSpaceBookingAssignment.Models;

namespace WorkSpaceBookingAssignment.Repository
{
    /// <summary>
    /// In-Memory implementation of IUserRepository
    /// 
    /// Use Cases:
    /// - Development/testing without database setup
    /// - Unit tests that need fast, isolated storage
    /// - Prototyping before adding database
    /// - Demos and POCs
    /// 
    /// Characteristics:
    /// - Data stored in memory (List<User>)
    /// - Data lost when application restarts
    /// - Thread-safe using SemaphoreSlim
    /// - No persistence to disk
    /// 
    /// SOLID Principles:
    /// - Single Responsibility: Only manages in-memory user collection
    /// - Open/Closed: Implements IUserRepository without modifying it
    /// - Liskov Substitution: Can replace PostgresUserRepository
    /// </summary>
    public class InMemoryUserRepository : IUserRepository, IDisposable
    {
        /// <summary>
        /// In-memory storage for users
        /// List<T> provides:
        /// - Fast access by index: O(1)
        /// - Linear search: O(n) - acceptable for small datasets
        /// - Not thread-safe by default - we handle with SemaphoreSlim
        /// 
        /// Alternative: ConcurrentDictionary<Guid, User> for better performance
        /// with many concurrent operations
        /// </summary>
        private readonly List<User> _users;

        /// <summary>
        /// SemaphoreSlim for thread synchronization in async methods
        /// 
        /// What is SemaphoreSlim?
        /// - Async-compatible synchronization primitive
        /// - Controls how many threads can access a resource simultaneously
        /// - (1, 1) means: only 1 thread at a time (like lock, but async-friendly)
        /// 
        /// Why not use 'lock' keyword?
        /// lock (_lockObject) { await ... } // ❌ DOESN'T COMPILE
        /// await _semaphore.WaitAsync(); // ✅ WORKS with async
        /// 
        /// How it works:
        /// Thread 1: await WaitAsync() → enters, count becomes 0
        /// Thread 2: await WaitAsync() → waits (count is 0)
        /// Thread 3: await WaitAsync() → waits (count is 0)
        /// Thread 1: Release() → exits, count becomes 1
        /// Thread 2: automatically proceeds (count becomes 0)
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Constructor initializes empty user list
        /// No database connection needed - just create List in memory
        /// </summary>
        public InMemoryUserRepository()
        {
            _users = new List<User>();
        }

        /// <summary>
        /// Creates a new user and adds to in-memory collection
        /// 
        /// Thread Safety Pattern:
        /// 1. await WaitAsync() - request exclusive access
        /// 2. try { ... } - do the work
        /// 3. finally { Release() } - ALWAYS release, even if exception occurs
        /// 
        /// Why check for duplicate email?
        /// - Simulates database unique constraint
        /// - Maintains data integrity
        /// - Provides consistent behavior with PostgreSQL implementation
        /// </summary>
        public async Task<User> CreateUserAsync(User user)
        {
            // Request exclusive access - waits if another thread is inside
            await _semaphore.WaitAsync();
            try
            {
                // Simulate database unique constraint on email
                // Case-insensitive comparison: John@Example.com == john@example.com
                var existingUser = _users.FirstOrDefault(u =>
                    u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));

                if (existingUser != null)
                {
                    throw new InvalidOperationException($"User with email {user.Email} already exists");
                }

                // Add to collection
                _users.Add(user);

                // Return the user (with Id, timestamps already set by User constructor)
                return user;
            }
            finally
            {
                // Release the semaphore - critical for avoiding deadlocks
                // If we forget this, all other threads wait forever!
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Retrieves user by ID from in-memory collection
        /// 
        /// Why still async even though List access is instant?
        /// - Consistent interface with database implementation
        /// - Allows swapping implementations without changing calling code
        /// - Future-proof: Can add async operations (logging, caching) later
        /// 
        /// Task.FromResult() wraps synchronous result in Task
        /// Alternative: Just return _users.FirstOrDefault(...) if method isn't async
        /// But keeping it async maintains interface compatibility
        /// </summary>
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            await _semaphore.WaitAsync();
            try
            {
                // Linear search through list - O(n)
                // For large collections, consider Dictionary<Guid, User> - O(1)
                return _users.FirstOrDefault(u => u.Id == id);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Returns all users from in-memory collection
        /// 
        /// Why ToList()?
        /// - Creates a copy of the list
        /// - Prevents external code from modifying internal _users collection
        /// - Snapshot at point in time - won't change if other threads modify _users
        /// 
        /// Example of problem without ToList():
        /// var users = _users; // Returns reference to internal list
        /// users.Clear(); // ❌ External code clears our internal storage!
        /// 
        /// With ToList():
        /// var users = _users.ToList(); // Returns new list (copy)
        /// users.Clear(); // ✅ Only clears the copy, _users unchanged
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                // Return a copy to prevent external modifications
                return _users.ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Updates an existing user's properties
        /// 
        /// Process:
        /// 1. Find user by ID
        /// 2. If not found, return null (let Service/Controller handle 404)
        /// 3. If found, copy new values to existing user
        /// 4. Use User.Update() method to ensure timestamp is updated
        /// 
        /// Why use Update() method instead of setting properties directly?
        /// - Encapsulation: User class manages its own update logic
        /// - Consistency: Timestamp always updated
        /// - Single Responsibility: User knows how to update itself
        /// </summary>
        public async Task<User?> UpdateUserAsync(Guid id, User user)
        {
            await _semaphore.WaitAsync();
            try
            {
                // Find existing user in collection
                var existingUser = _users.FirstOrDefault(u => u.Id == id);

                if (existingUser == null)
                {
                    return null;  // User not found
                }

                // Check if email is being changed to one that already exists
                // Allow keeping same email, but prevent taking someone else's email
                if (!existingUser.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailTaken = _users.Any(u =>
                        u.Id != id &&
                        u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));

                    if (emailTaken)
                    {
                        throw new InvalidOperationException($"Email {user.Email} is already in use");
                    }
                }

                // Use model's Update method - encapsulates update logic
                existingUser.Update(
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber
                );

                return existingUser;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Deletes a user from in-memory collection
        /// 
        /// Returns:
        /// - true: User was found and removed
        /// - false: User not found (nothing to delete)
        /// 
        /// List.Remove() returns bool:
        /// - true if item was in list and removed
        /// - false if item wasn't in list
        /// </summary>
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            await _semaphore.WaitAsync();
            try
            {
                var user = _users.FirstOrDefault(u => u.Id == id);

                if (user == null)
                {
                    return false;  // User not found
                }

                // Remove from list and return success
                _users.Remove(user);
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Finds user by email address (case-insensitive)
        /// 
        /// Why case-insensitive?
        /// - Email addresses are case-insensitive per RFC 5321
        /// - john@example.com == John@Example.com == JOHN@EXAMPLE.COM
        /// - Prevents duplicate accounts with different casing
        /// 
        /// StringComparison.OrdinalIgnoreCase:
        /// - Ordinal: Compare by character codes (fast)
        /// - IgnoreCase: Ignore upper/lower case differences
        /// - Alternative: CurrentCultureIgnoreCase (locale-aware, slower)
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            await _semaphore.WaitAsync();
            try
            {
                return _users.FirstOrDefault(u =>
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Cleanup method - releases SemaphoreSlim resources
        /// 
        /// Why implement IDisposable?
        /// - SemaphoreSlim holds unmanaged resources
        /// - Proper cleanup prevents resource leaks
        /// 
        /// When is this called?
        /// - When DI container disposes the repository (app shutdown)
        /// - Or manually: using var repo = new InMemoryUserRepository()
        /// 
        /// Pattern: Dispose pattern for proper resource cleanup
        /// </summary>
        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }
}
