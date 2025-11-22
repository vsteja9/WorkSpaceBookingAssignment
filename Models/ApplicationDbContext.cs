using Microsoft.EntityFrameworkCore;

namespace WorkSpaceBookingAssignment.Models
{
    /// <summary>
    /// Database context for Entity Framework Core
    /// 
    /// What is DbContext?
    /// - Represents a session with the database
    /// - Tracks changes to entities (insert, update, delete)
    /// - Translates LINQ queries to SQL
    /// - Manages database connections and transactions
    /// 
    /// Think of it as a "bridge" between your C# objects and database tables
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor receives configuration from dependency injection
        /// DbContextOptions contains:
        /// - Database provider (PostgreSQL, SQL Server, etc.)
        /// - Connection string
        /// - Logging configuration
        /// - Performance settings
        /// 
        /// Configured in Program.cs with:
        /// builder.Services.AddDbContext<ApplicationDbContext>(options => 
        ///     options.UseNpgsql(connectionString));
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)  // Pass options to base DbContext class
        {
        }

        /// <summary>
        /// DbSet represents the "users" table in PostgreSQL
        /// 
        /// Each DbSet property:
        /// - Maps to a database table
        /// - Provides CRUD operations: Add(), Remove(), Update()
        /// - Enables LINQ queries: Users.Where(u => u.Email == "...")
        /// 
        /// Usage examples:
        /// - Query: var users = await context.Users.ToListAsync();
        /// - Add: context.Users.Add(newUser); await context.SaveChangesAsync();
        /// - Update: user.FirstName = "New"; await context.SaveChangesAsync();
        /// - Delete: context.Users.Remove(user); await context.SaveChangesAsync();
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Fluent API for configuring the database schema
        /// 
        /// Why use Fluent API instead of Data Annotations?
        /// - More powerful: Can express complex relationships
        /// - Centralized: All config in one place
        /// - Separation of concerns: Keep model classes clean
        /// 
        /// When is this called?
        /// - During migration creation: dotnet ef migrations add InitialCreate
        /// - On first database access: EF validates the model
        /// 
        /// This method defines:
        /// - Table names and column types
        /// - Primary keys and foreign keys
        /// - Indexes for performance
        /// - Constraints (unique, check, default values)
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Users entity (table)
            modelBuilder.Entity<User>(entity =>
            {
                // Map entity to database table named "users" (lowercase follows PostgreSQL convention)
                // Without this, EF would create table named "Users" (PascalCase)
                entity.ToTable("users");

                // Define primary key (redundant here due to [Key] attribute, shown for clarity)
                // Composite keys: entity.HasKey(e => new { e.Id1, e.Id2 });
                entity.HasKey(e => e.Id);

                // Create unique index on Email column
                // Why index?
                // - Fast lookups: SELECT * FROM users WHERE email = 'x' uses index
                // - Unique constraint: Prevents duplicate emails at database level
                // 
                // Index naming: PostgreSQL convention uses idx_tablename_columnname
                entity.HasIndex(e => e.Email)
                    .IsUnique()  // UNIQUE constraint
                    .HasDatabaseName("idx_users_email");  // Custom index name

                // Configure Email property
                entity.Property(e => e.Email)
                    .IsRequired()           // NOT NULL constraint
                    .HasMaxLength(255);     // VARCHAR(255) in PostgreSQL

                // Configure FirstName property
                entity.Property(e => e.FirstName)
                    .IsRequired()           // Cannot be null
                    .HasMaxLength(100);     // Limit string length

                // Configure LastName property
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                // Configure PhoneNumber as optional (nullable)
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsRequired(false);     // NULL is allowed

                // Configure CreatedAt with database default value
                // CURRENT_TIMESTAMP is PostgreSQL function that returns current time
                // Benefit: Database sets value automatically if application doesn't provide it
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure UpdatedAt as nullable
                // Starts as NULL, gets value when user is first updated
                entity.Property(e => e.UpdatedAt)
                    .IsRequired(false);

                // Additional configurations you might add:
                // - Foreign keys: entity.HasOne(u => u.Department).WithMany(d => d.Users)
                // - Computed columns: entity.Property(e => e.FullName).HasComputedColumnSql("first_name || ' ' || last_name")
                // - Check constraints: entity.HasCheckConstraint("CK_User_Email", "length(email) > 5")
            });
        }
    }
}
