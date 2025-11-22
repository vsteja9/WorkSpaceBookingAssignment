# User Management System - Complete CRUD Implementation

This project demonstrates OOP principles and SOLID design patterns through a User Management System with two data storage options: In-Memory and PostgreSQL.

## 🏗️ Architecture

```
┌─────────────────┐
│   Controllers   │  (HTTP layer - handles requests/responses)
│  InMemoryUsers  │
│    DbUsers      │
└────────┬────────┘
         │ depends on
┌────────▼────────┐
│    Services     │  (Business logic layer)
│   UserService   │
└────────┬────────┘
         │ depends on
┌────────▼────────┐
│  Repositories   │  (Data access layer)
│    InMemory     │
│   PostgreSQL    │
└─────────────────┘
```

## 📁 Project Structure

```
WorkSpaceBookingAssignment/
├── Models/
│   ├── User.cs                      # Domain model with business logic
│   └── ApplicationDbContext.cs      # EF Core database context
├── DTOs/
│   └── UserDtos.cs                  # Data transfer objects
├── Repository/
│   ├── IUserRepository.cs           # Repository interface
│   ├── InMemoryUserRepository.cs    # In-memory implementation
│   └── PostgresUserRepository.cs    # PostgreSQL implementation
├── Services/
│   ├── IUserService.cs              # Service interface
│   └── UserService.cs               # Business logic implementation
├── Controllers/
│   ├── InMemoryUsersController.cs   # API for in-memory storage
│   └── DbUsersController.cs         # API for database storage
├── Program.cs                       # Application configuration
└── appsettings.json                 # Configuration settings
```

## 🎯 SOLID Principles Demonstrated

### Single Responsibility Principle (SRP)

- **Controllers**: Handle HTTP requests only
- **Services**: Contain business logic only
- **Repositories**: Handle data access only
- **Models**: Represent domain entities

### Open/Closed Principle (OCP)

- Can add new repository implementations (MySQL, MongoDB) without modifying existing code
- Can add new services without changing controllers

### Liskov Substitution Principle (LSP)

- `InMemoryUserRepository` and `PostgresUserRepository` are interchangeable
- Both implement `IUserRepository` interface

### Interface Segregation Principle (ISP)

- `IUserRepository` contains only necessary methods
- No unused methods forced on implementations

### Dependency Inversion Principle (DIP)

- Controllers depend on `IUserService` abstraction
- Services depend on `IUserRepository` abstraction
- No direct dependencies on concrete implementations

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 12+ (for database implementation)
- Docker (optional, for PostgreSQL)

### 1. Clone and Setup

```bash
cd /Users/vs185268/WorkSpaceBookingAssignment
dotnet restore
```

### 2. Configure PostgreSQL Connection

Update `appsettings.json` with your PostgreSQL credentials:

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=userdb;Username=postgres;Password=yourpassword"
  }
}
```

### 3. Start PostgreSQL (Docker)

```bash
docker run --name postgres-userdb \
  -e POSTGRES_PASSWORD=yourpassword \
  -e POSTGRES_DB=userdb \
  -p 5432:5432 \
  -d postgres:latest
```

Or use existing PostgreSQL installation.

### 4. Create Database Migration

```bash
dotnet ef migrations add InitialCreate
```

This creates migration files in `Migrations/` folder based on your models.

### 5. Apply Migration to Database

```bash
dotnet ef database update
```

This creates the `users` table in PostgreSQL with proper schema and indexes.

### 6. Run the Application

```bash
dotnet run
```

Application starts on:

- HTTP: http://localhost:5059
- HTTPS: https://localhost:5060

## 📡 API Endpoints

### In-Memory Storage (`/api/inmemory/users`)

| Method | Endpoint                   | Description     |
| ------ | -------------------------- | --------------- |
| POST   | `/api/inmemory/users`      | Create new user |
| GET    | `/api/inmemory/users`      | Get all users   |
| GET    | `/api/inmemory/users/{id}` | Get user by ID  |
| PUT    | `/api/inmemory/users/{id}` | Update user     |
| DELETE | `/api/inmemory/users/{id}` | Delete user     |

### Database Storage (`/api/db/users`)

| Method | Endpoint             | Description     |
| ------ | -------------------- | --------------- |
| POST   | `/api/db/users`      | Create new user |
| GET    | `/api/db/users`      | Get all users   |
| GET    | `/api/db/users/{id}` | Get user by ID  |
| PUT    | `/api/db/users/{id}` | Update user     |
| DELETE | `/api/db/users/{id}` | Delete user     |

## 📝 Example API Calls

### Create User

```bash
curl -X POST http://localhost:5059/api/inmemory/users \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "phoneNumber": "+1234567890"
  }'
```

**Response (201 Created):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "John",
  "lastName": "Doe",
  "fullName": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "+1234567890",
  "createdAt": "2025-11-22T10:30:00Z",
  "updatedAt": null
}
```

### Get All Users

```bash
curl http://localhost:5059/api/inmemory/users
```

**Response (200 OK):**

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "John",
    "lastName": "Doe",
    "fullName": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "+1234567890",
    "createdAt": "2025-11-22T10:30:00Z",
    "updatedAt": null
  }
]
```

### Get User by ID

```bash
curl http://localhost:5059/api/inmemory/users/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

### Update User

```bash
curl -X PUT http://localhost:5059/api/inmemory/users/3fa85f64-5717-4562-b3fc-2c963f66afa6 \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Smith",
    "email": "john.smith@example.com",
    "phoneNumber": "+0987654321"
  }'
```

### Delete User

```bash
curl -X DELETE http://localhost:5059/api/inmemory/users/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

## 🧪 Testing Both Implementations

### Test In-Memory (Data lost on restart)

```bash
# Create user
curl -X POST http://localhost:5059/api/inmemory/users -H "Content-Type: application/json" -d '{"firstName":"Test","lastName":"User","email":"test@example.com"}'

# Get all users
curl http://localhost:5059/api/inmemory/users
```

### Test Database (Data persists)

```bash
# Create user
curl -X POST http://localhost:5059/api/db/users -H "Content-Type: application/json" -d '{"firstName":"Test","lastName":"User","email":"test@example.com"}'

# Get all users
curl http://localhost:5059/api/db/users

# Restart app - data still exists
dotnet run
curl http://localhost:5059/api/db/users
```

## 🔄 Key Concepts Explained

### Async/Await

- All operations are asynchronous for scalability
- Doesn't block threads while waiting for I/O (database, network)
- Improves performance under high load

### Thread Safety

- `InMemoryUserRepository` uses `SemaphoreSlim` for thread-safe operations
- PostgreSQL handles concurrency through database transactions

### Validation

- **Model Validation**: `[Required]`, `[EmailAddress]` attributes
- **Business Validation**: Email uniqueness checked in service layer
- **Format Validation**: DTOs validate input format

### Error Handling

- `400 Bad Request`: Invalid input format
- `404 Not Found`: Resource doesn't exist
- `409 Conflict`: Business rule violation (duplicate email)

## 🎓 Learning Path

This project teaches:

1. **OOP Fundamentals**

   - Encapsulation (private setters, properties)
   - Abstraction (interfaces)
   - Inheritance (ControllerBase)
   - Polymorphism (IUserRepository implementations)

2. **SOLID Principles**

   - Each principle demonstrated with real examples
   - See comments in code for detailed explanations

3. **Design Patterns**

   - Repository Pattern (data access abstraction)
   - Dependency Injection (loose coupling)
   - Factory Pattern (service creation in Program.cs)
   - DTO Pattern (API data transfer)

4. **Entity Framework Core**

   - Code-First approach
   - Migrations
   - LINQ queries
   - Change tracking

5. **RESTful API Design**
   - HTTP verbs (GET, POST, PUT, DELETE)
   - Status codes (200, 201, 400, 404, 409)
   - Resource naming
   - Request/Response patterns

## 📚 Next Steps

1. **Add Authentication/Authorization**

   - JWT tokens
   - Role-based access control
   - User authentication

2. **Add Pagination**

   - Limit results per page
   - Improve performance for large datasets

3. **Add Caching**

   - Redis or in-memory cache
   - Cache frequently accessed data

4. **Add Logging**

   - Structured logging with Serilog
   - Track errors and performance

5. **Add Unit Tests**

   - xUnit or NUnit
   - Mock dependencies
   - Test business logic

6. **Add Integration Tests**
   - Test with real database
   - Test full request/response cycle

## 🛠️ Troubleshooting

### Port Already in Use

```bash
# Find process using port 5059
lsof -i :5059

# Kill the process
kill -9 <PID>
```

### PostgreSQL Connection Failed

- Verify PostgreSQL is running: `docker ps` or `pg_isready`
- Check connection string in `appsettings.json`
- Ensure database exists: `docker exec -it postgres-userdb psql -U postgres -c "\l"`

### Migration Errors

```bash
# Reset database
dotnet ef database drop
dotnet ef database update
```

## 📄 License

This is a learning project for educational purposes.
