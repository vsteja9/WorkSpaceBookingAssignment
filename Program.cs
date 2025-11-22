using Microsoft.EntityFrameworkCore;
using WorkSpaceBookingAssignment.Models;
using WorkSpaceBookingAssignment.Repository;
using WorkSpaceBookingAssignment.Services;

/// <summary>
/// Program.cs - Application entry point and configuration
/// 
/// This file demonstrates:
/// - Dependency Injection setup
/// - Service registration and lifetime management
/// - Middleware configuration
/// - Database context setup
/// - Controller routing
/// 
/// Execution Flow:
/// 1. CreateBuilder() - Initialize web application builder
/// 2. Add services to DI container (builder.Services.Add...)
/// 3. Build() - Create configured application
/// 4. Configure middleware pipeline (app.Use...)
/// 5. Run() - Start application and listen for requests
/// </summary>

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// SECTION 1: Service Registration (Dependency Injection)
/// 
/// What is Dependency Injection?
/// - Design pattern where objects receive their dependencies from external source
/// - Instead of: new UserService(new PostgresUserRepository())
/// - We do: Register services, let DI container create and inject them
/// 
/// Benefits:
/// - Loose coupling: Classes don't create their dependencies
/// - Testability: Easy to mock dependencies for unit tests
/// - Flexibility: Swap implementations without changing code
/// - Lifetime management: DI handles object creation and disposal
/// 
/// Service Lifetimes:
/// 
/// SINGLETON:
/// - Created once when first requested
/// - Same instance shared across entire application lifetime
/// - Use for: Stateless services, configuration, caching
/// - Example: HttpClient factory, logging, configuration
/// - Disposed: When application shuts down
/// 
/// SCOPED:
/// - Created once per HTTP request
/// - Same instance within a request, new instance per request
/// - Use for: DbContext, services that should be isolated per request
/// - Example: DbContext, user context, request-specific services
/// - Disposed: At end of HTTP request
/// 
/// TRANSIENT:
/// - Created every time it's requested
/// - New instance for each injection point
/// - Use for: Lightweight, stateless services
/// - Example: Helpers, formatters, validators
/// - Disposed: When scope ends or immediately after use
/// 
/// Thread Safety:
/// - Singleton: Must be thread-safe (multiple threads access same instance)
/// - Scoped: No thread safety needed (one request = one thread typically)
/// - Transient: No thread safety needed (not shared)
/// </summary>

// Add Controllers with API features (model validation, routing, etc.)
builder.Services.AddControllers();

// Add Swagger services for interactive API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/// <summary>
/// Database Configuration with Entity Framework Core
/// 
/// AddDbContext<ApplicationDbContext>:
/// - Registers DbContext as SCOPED service (new instance per request)
/// - Automatically disposed at end of request
/// - Connection pooling handled by EF Core
/// 
/// UseNpgsql():
/// - Configures PostgreSQL as database provider
/// - Connection string from appsettings.json
/// - Npgsql is PostgreSQL driver for .NET
/// 
/// Connection String Format:
/// "Host=localhost;Port=5432;Database=userdb;Username=postgres;Password=yourpassword"
/// 
/// Components:
/// - Host: Database server address (localhost for local, IP for remote)
/// - Port: PostgreSQL default port (5432)
/// - Database: Database name (created by migrations)
/// - Username: Database user account
/// - Password: Database password
/// 
/// Security Best Practices:
/// - Never hardcode connection strings in code
/// - Use User Secrets for development (dotnet user-secrets set)
/// - Use Environment Variables or Key Vault for production
/// - Encrypt connection strings in configuration files
/// 
/// Connection Pooling:
/// - EF Core reuses database connections for performance
/// - Minimum Pool Size: Keep connections open
/// - Maximum Pool Size: Limit concurrent connections
/// - Connection Lifetime: Recycle old connections
/// </summary>
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

/// <summary>
/// Repository Registration with Different Implementations
/// 
/// Strategy: Use factory pattern to provide different implementations
/// based on configuration or runtime decision
/// 
/// Why two repositories?
/// 1. Development: InMemoryUserRepository (no database needed)
/// 2. Production: PostgresUserRepository (persistent storage)
/// 3. Testing: InMemoryUserRepository (fast, isolated tests)
/// 
/// Keyed Services (.NET 8+):
/// - Register multiple implementations of same interface
/// - Inject by key: [FromKeyedServices("inmemory")] IUserRepository repo
/// 
/// Alternative without keyed services:
/// - Create separate service interfaces (IInMemoryUserService, IDbUserService)
/// - Or use factory pattern to decide at runtime
/// </summary>

// Register In-Memory Repository as Singleton
// Singleton because data should persist across requests within app lifetime
// (Even though data lost on restart, it's consistent within session)
builder.Services.AddSingleton<InMemoryUserRepository>();

// Register PostgreSQL Repository as Scoped
// Scoped because it uses DbContext which is scoped
// New repository instance per request, shares DbContext lifetime
builder.Services.AddScoped<PostgresUserRepository>();

/// <summary>
/// Service Registration with Keyed Services
/// 
/// Keyed Services (.NET 8+):
/// - Register multiple implementations of same interface with different keys
/// - Inject by key using [FromKeyedServices("keyname")]
/// - Allows same interface (IUserService) to have different implementations
/// 
/// Configuration:
/// InMemoryUsersController → UserService (inmemory key) → InMemoryUserRepository
/// DbUsersController → UserService (postgres key) → PostgresUserRepository
/// 
/// Benefits:
/// - Clean separation between in-memory and database implementations
/// - Both controllers use same IUserService interface
/// - Easy to swap or add new storage backends
/// </summary>

// Register both repositories with different keys
builder.Services.AddKeyedSingleton<IUserRepository, InMemoryUserRepository>("inmemory");
builder.Services.AddKeyedScoped<IUserRepository, PostgresUserRepository>("postgres");

// Register two different UserService instances with keys
builder.Services.AddKeyedScoped<IUserService, UserService>("inmemory", (sp, key) =>
{
    var repo = sp.GetRequiredKeyedService<IUserRepository>("inmemory");
    return new UserService(repo);
});

builder.Services.AddKeyedScoped<IUserService, UserService>("postgres", (sp, key) =>
{
    var repo = sp.GetRequiredKeyedService<IUserRepository>("postgres");
    return new UserService(repo);
});

/// <summary>
/// Build Application
/// 
/// Creates WebApplication instance with all configured services
/// Services are now registered and ready for injection
/// Next: Configure HTTP request pipeline (middleware)
/// </summary>
var app = builder.Build();

/// <summary>
/// SECTION 2: Middleware Pipeline Configuration
/// 
/// What is Middleware?
/// - Components that handle HTTP requests/responses
/// - Execute in sequence (pipeline)
/// - Each can process request, call next middleware, process response
/// 
/// Order matters!
/// - Request flows top to bottom
/// - Response flows bottom to top
/// 
/// Example flow:
/// Request → UseHttpsRedirection → UseRouting → MapControllers → Controller
/// Response ← UseHttpsRedirection ← UseRouting ← MapControllers ← Controller
/// 
/// Common middleware:
/// - Authentication: Verify user identity
/// - Authorization: Check user permissions
/// - Routing: Match URL to controller/action
/// - CORS: Allow cross-origin requests
/// - Static Files: Serve HTML, CSS, JS, images
/// - Exception Handling: Catch and format errors
/// </summary>

// Enable Swagger in development environment only
// Provides interactive API documentation at /swagger
if (app.Environment.IsDevelopment())
{
    /// <summary>
    /// Swagger UI Configuration
    /// 
    /// Provides interactive web interface to test API endpoints
    /// Access at: http://localhost:5059/swagger
    /// 
    /// Features:
    /// - List all endpoints with descriptions
    /// - Try API calls directly from browser
    /// - See request/response examples
    /// - View data models and validation rules
    /// - Test authentication (when implemented)
    /// </summary>
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
        options.RoutePrefix = "swagger"; // Access at /swagger
    });
}

// HTTPS Redirection: Redirect HTTP to HTTPS
// Important for security: Encrypts data in transit
app.UseHttpsRedirection();

/// <summary>
/// Routing Middleware
/// 
/// Maps incoming requests to appropriate controllers/actions
/// Based on route patterns defined in [Route] attributes
/// 
/// Examples:
/// POST /api/inmemory/users → InMemoryUsersController.CreateUser
/// GET /api/db/users/123 → DbUsersController.GetUserById
/// </summary>

// Map controller endpoints (RESTful API routes)
app.MapControllers();

/// <summary>
/// Run Application
/// 
/// Starts the web server and begins listening for requests
/// Blocks until application is shut down (Ctrl+C)
/// 
/// Default URLs:
/// - http://localhost:5000
/// - https://localhost:5001
/// 
/// Override in launchSettings.json or appsettings.json:
/// "Kestrel": {
///   "Endpoints": {
///     "Http": { "Url": "http://localhost:8080" }
///   }
/// }
/// </summary>
app.Run();

/// <summary>
/// Next Steps After Configuration:
/// 
/// 1. Create Database Migration:
///    dotnet ef migrations add InitialCreate
///    - Generates C# code to create database schema
///    - Based on ApplicationDbContext and User model
///    - Creates Migrations folder with migration files
/// 
/// 2. Apply Migration to Database:
///    dotnet ef database update
///    - Executes migration against PostgreSQL
///    - Creates 'users' table with columns
///    - Creates indexes (email unique index)
/// 
/// 3. Test API Endpoints:
///    In-Memory:
///    POST http://localhost:5000/api/inmemory/users
///    GET  http://localhost:5000/api/inmemory/users
///    
///    Database:
///    POST http://localhost:5000/api/db/users
///    GET  http://localhost:5000/api/db/users
/// 
/// 4. View OpenAPI Documentation:
///    Navigate to http://localhost:5000/openapi/v1.json
///    Or use Swagger UI (if enabled above)
/// 
/// 5. Docker PostgreSQL Setup:
///    docker run --name postgres-userdb \
///      -e POSTGRES_PASSWORD=yourpassword \
///      -e POSTGRES_DB=userdb \
///      -p 5432:5432 \
///      -d postgres:latest
/// </summary>


