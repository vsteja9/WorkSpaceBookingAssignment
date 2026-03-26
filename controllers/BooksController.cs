using BooksDtos;
using Microsoft.AspNetCore.Mvc;
using WorkSpaceBookingAssignment.DTOs;
using WorkSpaceBookingAssignment.Services;

namespace WorkSpaceBookingAssignment.Controllers
{
    /// <summary>
    /// Books API Controller
    /// Handles all HTTP requests for book management
    /// </summary>
    [ApiController]  // Enables automatic model validation and better error responses
    [Route("api/books")]  // Base route: /api/books
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;

        public BooksController(IBooksService booksService)
        {
            _booksService = booksService;
        }

        /// <summary>
        /// Create a new book
        /// POST /api/books
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BookResponse>> CreateBook([FromBody] CreateBookRequest request)
        {
            try
            {
                var book = await _booksService.CreateBookAsync(request);
                return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Get a book by ID
        /// GET /api/books/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponse>> GetBookById(Guid id)
        {
            try
            {
                var book = await _booksService.GetBookByIdAsync(id);
                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Get all books
        /// GET /api/books
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<BookResponse>>> GetAllBooks()
        {
            try
            {
                var books = await _booksService.GetBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Update a book
        /// PUT /api/books/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<BookResponse>> UpdateBook(Guid id, [FromBody] updateBook request)
        {
            try
            {
                var book = await _booksService.UpdateBookAsync(id, request);
                return Ok(book);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Delete a book
        /// DELETE /api/books/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(Guid id)
        {
            try
            {
                var result = await _booksService.DeleteBookAsync(id);
                if (result)
                {
                    return NoContent();  // 204 No Content - successful deletion
                }
                return NotFound(new ErrorResponse($"Book with ID {id} not found"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }
    }
}