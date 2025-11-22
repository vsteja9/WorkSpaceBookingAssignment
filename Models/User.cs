using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkSpaceBookingAssignment.Models
{
    /// <summary>
    /// User entity - represents a user in the system
    /// 
    /// OOP Principles Demonstrated:
    /// - Encapsulation: Private setters for Id, CreatedAt protect data integrity
    /// - Abstraction: Public methods hide internal timestamp management
    /// - Data validation: Attributes ensure data quality
    /// </summary>
    public class User
    {
        /// <summary>
        /// Primary key - Guid provides globally unique identifiers
        /// [Key] tells EF Core this is the primary key
        /// [DatabaseGenerated(None)] means application generates the ID, not database
        /// Private setter prevents external modification after creation
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; }

        /// <summary>
        /// User's first name
        /// [Required] creates NOT NULL constraint in database
        /// [MaxLength(100)] limits string size and creates VARCHAR(100) in PostgreSQL
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name
        /// Same validation as FirstName
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Email address - must be unique across all users
        /// [EmailAddress] validates email format
        /// Unique constraint is defined in DbContext using Fluent API
        /// </summary>
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Phone number - optional field (nullable)
        /// Format: +1234567890 (includes country code)
        /// </summary>
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Audit field - when user was created
        /// Private setter enforces immutability - only set once at creation
        /// Always stored as UTC to avoid timezone issues
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Audit field - when user was last modified
        /// Nullable: null = never updated, has value = last update time
        /// Private setter controlled through UpdateTimestamp() method
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Parameterless constructor required by Entity Framework Core
        /// EF uses reflection to create instances when reading from database
        /// Don't use this constructor in your code - use the parameterized one
        /// </summary>
        public User()
        {
            // EF Core will populate properties after instantiation
        }

        /// <summary>
        /// Constructor for creating new users in application code
        /// Automatically generates new Guid and sets creation timestamp
        /// 
        /// Usage: var user = new User("John", "Doe", "john@example.com", "+1234567890");
        /// </summary>
        /// <param name="firstName">User's first name</param>
        /// <param name="lastName">User's last name</param>
        /// <param name="email">User's email address</param>
        /// <param name="phoneNumber">Optional phone number with country code</param>
        public User(string firstName, string lastName, string email, string? phoneNumber = null)
        {
            Id = Guid.NewGuid();  // Generate unique identifier
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            CreatedAt = DateTime.UtcNow;  // Always use UTC for consistency
            // UpdatedAt remains null for new users
        }

        /// <summary>
        /// Updates the modification timestamp to current UTC time
        /// Call this method whenever user properties are changed
        /// 
        /// Example: user.Email = "newemail@example.com"; user.UpdateTimestamp();
        /// </summary>
        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Returns user's full name by combining first and last name
        /// Demonstrates encapsulation - formatting logic is hidden in the model
        /// 
        /// Example: user.GetFullName() returns "John Doe"
        /// </summary>
        public string GetFullName()
        {
            return $"{FirstName} {LastName}".Trim();
        }

        /// <summary>
        /// Updates user properties with new values
        /// Single method to update all modifiable fields (Single Responsibility Principle)
        /// Automatically updates the UpdatedAt timestamp
        /// 
        /// This is better than updating properties individually because:
        /// - Ensures timestamp is always updated
        /// - Groups related changes in one transaction
        /// - Provides single point for update validation if needed
        /// </summary>
        public void Update(string firstName, string lastName, string email, string? phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            UpdateTimestamp();  // Automatically track when update occurred
        }
    }
}
