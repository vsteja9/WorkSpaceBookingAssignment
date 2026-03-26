using System.ComponentModel.DataAnnotations;
using WorkSpaceBookingAssignment.Models;

namespace BooksDtos
{
    /// <summary>
    /// Response DTO for returning book data to client
    /// </summary>
    public record BookResponse
    {
        public Guid Id { get; init; }
        public string BookName { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public double Cost { get; init; }

        /// <summary>
        /// Factory method to create BookResponse from Book entity
        /// </summary>
        public static BookResponse FromBook(Book book)
        {
            return new BookResponse
            {
                Id = book.id,
                BookName = book.BookName,
                Author = book.Author,
                Cost = book.Cost
            };
        }
    }

    public class CreateBookRequest
    {
        [Required(ErrorMessage = "Name is Required")]
        public string BookName { get; set; } = string.Empty;

        [Required(ErrorMessage = "author is Required")]
        public string Author { get; set; } = string.Empty;
        [Required(ErrorMessage = "Cost is Required")]
        public double Cost { get; set; } = 0.0;
    }

    public record updateBook
    {
        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Cost is required")]
        public double Cost { get; set; }
    }
}