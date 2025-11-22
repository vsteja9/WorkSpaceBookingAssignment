using Microsoft.AspNetCore.Mvc;
using WorkSpaceBookingAssignment.DTOs;
using WorkSpaceBookingAssignment.Services;

namespace WorkSpaceBookingAssignment.Controllers
{
    /// <summary>
    /// Controller for In-Memory User Management
    /// 
    /// Route: /api/inmemory/users
    /// 
    /// What is a Controller?
    /// - Entry point for HTTP requests
    /// - Maps HTTP verbs (GET, POST, PUT, DELETE) to methods
    /// - Handles HTTP-specific concerns (status codes, headers)
    /// - Converts between DTOs (JSON) and domain models
    /// 
    /// Controller Responsibilities (ONLY these):
    /// 1. Receive HTTP request
    /// 2. Validate request format (DTOs handle this with attributes)
    /// 3. Call appropriate service method
    /// 4. Map result to response DTO
    /// 5. Return appropriate HTTP status code
    /// 
    /// Controller Should NOT:
    /// - Contain business logic (that's in Service)
    /// - Access database directly (that's in Repository)
    /// - Make complex decisions (delegate to Service)
    /// 
    /// SOLID Principles:
    /// - Single Responsibility: Only handles HTTP concerns
    /// - Dependency Inversion: Depends on IUserService, not UserService
    /// - Open/Closed: Can add endpoints without modifying existing ones
    /// 
    /// Attributes Explained:
    /// [ApiController]: Enables automatic model validation, binding, and error handling
    /// [Route]: Defines URL pattern for all endpoints in this controller
    /// [HttpGet], [HttpPost], etc.: Maps HTTP verb to method
    /// [FromBody]: Binds JSON request body to parameter
    /// [FromRoute]: Binds URL path parameter to method parameter
    /// </summary>
    [ApiController]  // Enables API-specific features (auto validation, etc.)
    [Route("api/inmemory/users")]  // Base route for all actions in this controller
    public class InMemoryUsersController : ControllerBase
    {
        /// <summary>
        /// Service dependency injected via constructor
        /// 
        /// Why inject IUserService, not IUserRepository?
        /// - Controller talks to Service, never directly to Repository
        /// - Service contains business logic
        /// - Repository is implementation detail hidden by Service
        /// 
        /// Layered Architecture:
        /// Client → Controller → Service → Repository → Database
        /// 
        /// Each layer only talks to the layer directly below:
        /// - Controller knows about Service (not Repository)
        /// - Service knows about Repository (not Database)
        /// - Repository knows about Database (not HTTP)
        /// 
        /// This service uses InMemoryUserRepository (configured in Program.cs)
        /// 
        /// [FromKeyedServices("inmemory")]:
        /// - .NET 8+ feature for named dependency injection
        /// - Tells DI to inject the service registered with key "inmemory"
        /// - Allows multiple implementations of same interface
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor with keyed dependency injection
        /// 
        /// ASP.NET Core DI automatically:
        /// 1. Sees InMemoryUsersController needs IUserService with key "inmemory"
        /// 2. Looks up keyed registration in Program.cs
        /// 3. Creates UserService with InMemoryUserRepository
        /// 4. Injects into this constructor
        /// 5. Creates controller instance
        /// 6. Routes request to appropriate action method
        /// </summary>
        public InMemoryUsersController([FromKeyedServices("inmemory")] IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Creates a new user
        /// 
        /// HTTP: POST /api/inmemory/users
        /// Request Body (JSON):
        /// {
        ///   "firstName": "John",
        ///   "lastName": "Doe",
        ///   "email": "john@example.com",
        ///   "phoneNumber": "+1234567890"
        /// }
        /// 
        /// Success Response: 201 Created
        /// {
        ///   "id": "guid...",
        ///   "firstName": "John",
        ///   "lastName": "Doe",
        ///   "fullName": "John Doe",
        ///   "email": "john@example.com",
        ///   "phoneNumber": "+1234567890",
        ///   "createdAt": "2025-11-22T10:30:00Z",
        ///   "updatedAt": null
        /// }
        /// Location Header: /api/inmemory/users/guid-value
        /// 
        /// Error Responses:
        /// 400 Bad Request: Invalid input (missing fields, format errors)
        /// 409 Conflict: Email already exists
        /// 
        /// [HttpPost] Attribute:
        /// - Maps HTTP POST requests to this method
        /// - Only POST requests will invoke this method
        /// 
        /// [FromBody] Attribute:
        /// - Tells ASP.NET to deserialize JSON body to CreateUserRequest
        /// - Automatically validates using Data Annotations ([Required], etc.)
        /// - If validation fails, returns 400 automatically (thanks to [ApiController])
        /// 
        /// CreatedAtAction():
        /// - Returns 201 Created status
        /// - Sets Location header to new resource URL
        /// - Includes resource in response body
        /// - First parameter: Action name to generate URL
        /// - Second parameter: Route values for URL generation
        /// - Third parameter: Response body
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                // Call service to create user
                // Service handles business validation (email uniqueness)
                var user = await _userService.CreateUserAsync(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber
                );

                // Convert domain model (User) to response DTO (UserResponse)
                // Why? Control what data is exposed, add computed fields (FullName)
                var response = UserResponse.FromUser(user);

                // Return 201 Created with Location header
                // Location: /api/inmemory/users/{id}
                // Client can use this URL to access the new resource
                return CreatedAtAction(
                    nameof(GetUserById),  // Action name for URL generation
                    new { id = user.Id }, // Route values
                    response              // Response body
                );
            }
            catch (InvalidOperationException ex)
            {
                // Business rule violation (duplicate email)
                // Return 409 Conflict with error message
                return Conflict(new ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                // Invalid input (though DTOs should catch this)
                // Return 400 Bad Request with error message
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a user by ID
        /// 
        /// HTTP: GET /api/inmemory/users/{id}
        /// Example: GET /api/inmemory/users/a1b2c3d4-e5f6-7890-abcd-ef1234567890
        /// 
        /// Success Response: 200 OK
        /// {
        ///   "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        ///   "firstName": "John",
        ///   ...
        /// }
        /// 
        /// Error Response: 404 Not Found
        /// {
        ///   "message": "User not found"
        /// }
        /// 
        /// [HttpGet("{id}")] Attribute:
        /// - Maps GET /api/inmemory/users/{id} to this method
        /// - {id} is a route parameter
        /// - Automatically bound to 'id' parameter (by name matching)
        /// - ASP.NET converts string in URL to Guid automatically
        /// - If conversion fails (invalid Guid format), returns 400 automatically
        /// 
        /// Why no [FromRoute] attribute?
        /// - Not needed: ASP.NET infers from route template
        /// - Can be explicit: GetUserById([FromRoute] Guid id) for clarity
        /// 
        /// Ok() vs NotFound():
        /// - Ok(data): Returns 200 with data in body
        /// - NotFound(): Returns 404 with no body
        /// - NotFound(data): Returns 404 with error message in body
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                // User not found: Return 404
                return NotFound(new ErrorResponse("User not found"));
            }

            // Convert to DTO and return 200
            var response = UserResponse.FromUser(user);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves all users
        /// 
        /// HTTP: GET /api/inmemory/users
        /// 
        /// Success Response: 200 OK
        /// [
        ///   { "id": "...", "firstName": "John", ... },
        ///   { "id": "...", "firstName": "Jane", ... }
        /// ]
        /// 
        /// Empty Response: 200 OK
        /// []
        /// 
        /// [HttpGet] with no template:
        /// - Maps GET /api/inmemory/users (base route)
        /// - No route parameters needed
        /// 
        /// Why always return 200, even for empty list?
        /// - Empty list is valid data, not an error
        /// - Client can differentiate: [] vs error response
        /// - RESTful convention: 200 for successful GET, even if empty
        /// 
        /// Performance Note:
        /// This loads ALL users - problematic for large datasets
        /// Production should use pagination:
        /// GET /api/inmemory/users?page=1&pageSize=20
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            // Convert each User to UserResponse
            // Select() projects each item using FromUser factory method
            var response = users.Select(UserResponse.FromUser).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Updates an existing user
        /// 
        /// HTTP: PUT /api/inmemory/users/{id}
        /// Request Body (JSON):
        /// {
        ///   "firstName": "John Updated",
        ///   "lastName": "Doe Updated",
        ///   "email": "john.updated@example.com",
        ///   "phoneNumber": "+0987654321"
        /// }
        /// 
        /// Success Response: 200 OK
        /// {
        ///   "id": "...",
        ///   "firstName": "John Updated",
        ///   ...
        ///   "updatedAt": "2025-11-22T11:00:00Z"
        /// }
        /// 
        /// Error Responses:
        /// 400 Bad Request: Invalid input
        /// 404 Not Found: User doesn't exist
        /// 409 Conflict: Email taken by another user
        /// 
        /// PUT vs PATCH:
        /// - PUT: Replace entire resource (all fields required)
        /// - PATCH: Partial update (only send changed fields)
        /// 
        /// This is PUT: Client must send all fields
        /// For PATCH, we'd use:
        /// [HttpPatch("{id}")]
        /// public async Task<IActionResult> PartialUpdateUser(
        ///     Guid id, 
        ///     [FromBody] JsonPatchDocument<UpdateUserRequest> patchDoc)
        /// 
        /// Example PATCH request:
        /// [
        ///   { "op": "replace", "path": "/firstName", "value": "NewName" }
        /// ]
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(
            Guid id,
            [FromBody] UpdateUserRequest request)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(
                    id,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber
                );

                if (user == null)
                {
                    // User not found
                    return NotFound(new ErrorResponse("User not found"));
                }

                // Convert to DTO and return 200
                var response = UserResponse.FromUser(user);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                // Business rule violation (email conflict)
                return Conflict(new ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                // Invalid input
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Deletes a user
        /// 
        /// HTTP: DELETE /api/inmemory/users/{id}
        /// 
        /// Success Response: 204 No Content
        /// (No body returned - deletion confirmed by status code)
        /// 
        /// Error Response: 404 Not Found
        /// {
        ///   "message": "User not found"
        /// }
        /// 
        /// [HttpDelete("{id}")]:
        /// - Maps DELETE requests to this method
        /// - {id} route parameter bound to method parameter
        /// 
        /// NoContent() vs Ok():
        /// - NoContent(): 204, no response body (standard for DELETE)
        /// - Ok(): 200, can include body if needed
        /// 
        /// RESTful DELETE conventions:
        /// - 204 No Content: Successfully deleted, no body needed
        /// - 200 OK: Successfully deleted, return deleted resource
        /// - 202 Accepted: Deletion queued but not complete yet
        /// - 404 Not Found: Resource doesn't exist
        /// 
        /// Idempotency:
        /// - First DELETE: Returns 204 (deleted)
        /// - Second DELETE: Returns 404 (already gone)
        /// - Both are successful outcomes for client
        /// - Client gets desired state: resource doesn't exist
        /// 
        /// Alternative: Return 204 for both (idempotent DELETE)
        /// Some APIs do this - "doesn't exist" = successful deletion
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                // User not found
                return NotFound(new ErrorResponse("User not found"));
            }

            // Successfully deleted - return 204 No Content
            return NoContent();
        }
    }
}
