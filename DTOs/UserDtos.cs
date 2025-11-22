using System.ComponentModel.DataAnnotations;

namespace WorkSpaceBookingAssignment.DTOs
{
    /// <summary>
    /// Data Transfer Objects (DTOs) for User API
    /// 
    /// What are DTOs?
    /// - Objects specifically designed for transferring data between layers
    /// - Separate from domain models (User entity)
    /// 
    /// Why use DTOs instead of directly exposing User model?
    /// 1. Security: Hide internal fields (Id generation, timestamps)
    /// 2. Flexibility: API contract independent from database schema
    /// 3. Validation: Different validation rules for create vs update
    /// 4. Versioning: Can change database without breaking API
    /// 
    /// Example: User model has CreatedAt, but CreateUserRequest doesn't
    /// because clients shouldn't set creation time - server does.
    /// </summary>
    /// 
    /// <summary>
    /// Request DTO for creating a new user
    /// 
    /// Why separate from User model?
    /// - Client doesn't provide Id (server generates it)
    /// - Client doesn't set CreatedAt/UpdatedAt (server manages timestamps)
    /// - Validation rules specific to creation (e.g., email must not exist)
    /// 
    /// Usage in Controller:
    /// [HttpPost]
    /// public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    /// </summary>
    public record CreateUserRequest
    {
        /// <summary>
        /// User's first name
        /// [Required] ensures client must provide this value
        /// [MinLength] prevents empty strings like " "
        /// [MaxLength] matches database constraint
        /// </summary>
        [Required(ErrorMessage = "First name is required")]
        [MinLength(1, ErrorMessage = "First name cannot be empty")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; init; } = string.Empty;

        /// <summary>
        /// User's last name
        /// Same validation as FirstName
        /// 'init' keyword: Property can only be set during object initialization
        /// Supports: new CreateUserRequest { FirstName = "John", LastName = "Doe" }
        /// Prevents: request.FirstName = "Jane" after creation (immutable)
        /// </summary>
        [Required(ErrorMessage = "Last name is required")]
        [MinLength(1, ErrorMessage = "Last name cannot be empty")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; init; } = string.Empty;

        /// <summary>
        /// User's email address
        /// [EmailAddress] validates format: must contain @ and valid domain
        /// Service layer will check if email already exists
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Optional phone number with country code
        /// Not required - client can omit this field
        /// Validation only applies if value is provided
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? PhoneNumber { get; init; }
    }

    /// <summary>
    /// Request DTO for updating an existing user
    /// 
    /// Why separate from CreateUserRequest?
    /// - Update might have different validation rules
    /// - Some fields might be updateable but not settable during creation
    /// - Clear intent: This is for updates, not creation
    /// 
    /// Note: In this simple example, it's identical to CreateUserRequest
    /// In real apps, you might allow partial updates or have different rules
    /// </summary>
    public record UpdateUserRequest
    {
        [Required(ErrorMessage = "First name is required")]
        [MinLength(1, ErrorMessage = "First name cannot be empty")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; init; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [MinLength(1, ErrorMessage = "Last name cannot be empty")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; init; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; init; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? PhoneNumber { get; init; }
    }

    /// <summary>
    /// Response DTO for returning user data to client
    /// 
    /// Why not return User entity directly?
    /// - Control what data is exposed (hide sensitive fields if any)
    /// - Format data for client consumption (e.g., format dates)
    /// - Prevent over-posting: Client can't send back fields that shouldn't change
    /// 
    /// This DTO includes all user data that's safe to expose to clients
    /// </summary>
    public record UserResponse
    {
        /// <summary>
        /// User's unique identifier
        /// Exposed to client for subsequent requests (GET /api/users/{id})
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; init; } = string.Empty;

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; init; } = string.Empty;

        /// <summary>
        /// Full name computed from first and last name
        /// Convenience field for client - avoids concatenation on frontend
        /// </summary>
        public string FullName { get; init; } = string.Empty;

        /// <summary>
        /// User's email address
        /// </summary>
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// User's phone number (if provided)
        /// </summary>
        public string? PhoneNumber { get; init; }

        /// <summary>
        /// When the user was created
        /// Useful for UI display and auditing
        /// </summary>
        public DateTime CreatedAt { get; init; }

        /// <summary>
        /// When the user was last updated
        /// null = never updated since creation
        /// </summary>
        public DateTime? UpdatedAt { get; init; }

        /// <summary>
        /// Factory method to create UserResponse from User entity
        /// 
        /// Why use a factory method?
        /// - Centralized mapping logic
        /// - Type-safe conversion
        /// - Easy to add computed fields (like FullName)
        /// 
        /// Usage: 
        /// var response = UserResponse.FromUser(user);
        /// return Ok(response);
        /// </summary>
        public static UserResponse FromUser(Models.User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.GetFullName(),  // Use model's business logic
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }

    /// <summary>
    /// Standard error response for API
    /// 
    /// Provides consistent error format across all endpoints:
    /// {
    ///   "message": "User with email already exists",
    ///   "errors": ["Email must be unique"]
    /// }
    /// 
    /// Benefits:
    /// - Clients can parse errors consistently
    /// - Support for multiple error messages (validation failures)
    /// - Clear separation between user message and technical details
    /// </summary>
    public record ErrorResponse
    {
        /// <summary>
        /// Human-readable error message
        /// Example: "Failed to create user"
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Optional detailed error messages
        /// Example: ["Email is required", "Phone number is invalid"]
        /// Useful for validation errors where multiple fields might be wrong
        /// </summary>
        public List<string>? Errors { get; init; }

        /// <summary>
        /// Constructor for simple error with just a message
        /// </summary>
        public ErrorResponse(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Constructor for error with multiple validation messages
        /// </summary>
        public ErrorResponse(string message, List<string> errors)
        {
            Message = message;
            Errors = errors;
        }
    }
}
