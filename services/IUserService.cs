using WorkSpaceBookingAssignment.Models;

namespace WorkSpaceBookingAssignment.Services
{
    /// <summary>
    /// Service interface - defines business logic contract for user operations
    /// 
    /// Service Layer Responsibilities (vs Repository vs Controller):
    /// 
    /// REPOSITORY:
    /// - Data access only (CRUD operations)
    /// - No business rules
    /// - Example: "Save this user to database"
    /// 
    /// SERVICE (this layer):
    /// - Business logic and rules
    /// - Data validation beyond format (email uniqueness, business constraints)
    /// - Orchestrates multiple repositories if needed
    /// - Example: "Create user, but only if email is unique and user is valid"
    /// 
    /// CONTROLLER:
    /// - HTTP concerns (status codes, routing)
    /// - Request/response mapping (DTOs to Models)
    /// - No business logic
    /// - Example: "Receive POST request, call service, return 201 Created"
    /// 
    /// Why separate Service from Repository?
    /// 
    /// SINGLE RESPONSIBILITY:
    /// - Repository: How to access data
    /// - Service: What business rules to enforce
    /// 
    /// Example showing difference:
    /// Repository: Can save any user (even with duplicate email)
    /// Service: Checks email uniqueness BEFORE calling repository
    /// 
    /// TESTABILITY:
    /// - Test business logic without database
    /// - Mock repository in service tests
    /// - Test repository with in-memory database
    /// 
    /// REUSABILITY:
    /// - Same service logic for API, CLI, background jobs
    /// - Repository can be reused by multiple services
    /// 
    /// Example of complex business logic:
    /// - User can only be deleted if they have no active orders
    /// - Repository doesn't know about orders
    /// - Service checks orders before calling repository.DeleteUser()
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user with business validation
        /// 
        /// Business Rules Enforced:
        /// 1. Email must be unique (check via repository)
        /// 2. All required fields present (enforced by model)
        /// 3. Email format valid (enforced by model attributes)
        /// 
        /// Returns: Created user with generated ID
        /// Throws: InvalidOperationException if email exists
        /// Throws: ArgumentException if validation fails
        /// </summary>
        Task<User> CreateUserAsync(string firstName, string lastName, string email, string? phoneNumber);

        /// <summary>
        /// Retrieves user by ID
        /// 
        /// Why in Service if it's just a pass-through to Repository?
        /// - Consistent interface: All user operations go through service
        /// - Future-proof: Can add caching, logging, authorization later
        /// - Example addition: Log user access for audit trail
        /// 
        /// Returns: User if found, null otherwise
        /// </summary>
        Task<User?> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all users
        /// 
        /// Potential enhancements:
        /// - Filtering: GetUsersAsync(string? searchTerm)
        /// - Pagination: GetUsersAsync(int page, int pageSize)
        /// - Sorting: GetUsersAsync(string sortBy, bool descending)
        /// - Authorization: Only return users current user has permission to see
        /// 
        /// Returns: List of all users
        /// </summary>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Updates user with business validation
        /// 
        /// Business Rules:
        /// 1. User must exist
        /// 2. If changing email, new email must be unique
        /// 3. Cannot change to invalid data
        /// 
        /// Example additional rule:
        /// - Can only update your own profile (check current user ID)
        /// - Admin can update any profile
        /// 
        /// Returns: Updated user if successful, null if user not found
        /// Throws: InvalidOperationException if email is taken by another user
        /// </summary>
        Task<User?> UpdateUserAsync(Guid id, string firstName, string lastName, string email, string? phoneNumber);

        /// <summary>
        /// Deletes user with business validation
        /// 
        /// Business Rules (examples):
        /// 1. User must exist
        /// 2. Cannot delete yourself (current logged-in user)
        /// 3. Cannot delete user with active orders
        /// 4. Admin permission required
        /// 
        /// Soft Delete Alternative:
        /// Task<bool> SoftDeleteUserAsync(Guid id, Guid deletedByUserId);
        /// - Marks user as deleted instead of removing
        /// - Records who deleted and when
        /// - Can be restored later
        /// 
        /// Returns: true if deleted, false if user not found
        /// </summary>
        Task<bool> DeleteUserAsync(Guid id);
    }
}
