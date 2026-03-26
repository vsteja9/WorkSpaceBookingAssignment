using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BooksDtos;

namespace WorkSpaceBookingAssignment.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        public string BookName { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public double Cost { get; set; }

        // Parameterless constructor for EF Core
        public Book()
        {
        }

        // Constructor to create from DTO
        public Book(CreateBookRequest createBook)
        {
            BookName = createBook.BookName;
            Author = createBook.Author;
            Cost = createBook.Cost;
        }
    }

}
