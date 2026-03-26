using BooksDtos;
using Microsoft.EntityFrameworkCore;
using WorkSpaceBookingAssignment.Models;

namespace WorkSpaceBookingAssignment.Repository
{
    public interface IBooksRepository
    {
        Task<Book> createBook(CreateBookRequest createBook);
        Task<Book> getBookById(Guid id);
        Task<List<Book>> getBooks();
        Task<Book> updateBook(Guid id, updateBook updateBook);
        Task<bool> deleteBook(Guid id);

    }

    public class BooksRepository : IBooksRepository
    {
        public readonly ApplicationDbContext _dbContext;
        public BooksRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public async Task<Book> createBook(CreateBookRequest createBook)
        {
            Book book = new Book
            {
                BookName = createBook.BookName,
                Author = createBook.Author,
                Cost = createBook.Cost,
            };
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
            return book;
        }

        public async Task<bool> deleteBook(Guid id)
        {
            Book? book = await _dbContext.Books.FindAsync(id);
            if (book == null) throw new Exception("Book notFound");
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Book> getBookById(Guid id)
        {
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.id == id);
            if (book == null) throw new KeyNotFoundException($"Book with ID {id} not found");
            return book;
        }

        public async Task<List<Book>> getBooks()
        {
            return await _dbContext.Books.ToListAsync();
        }

        public async Task<Book> updateBook(Guid id, updateBook updateBook)
        {
            var book = await _dbContext.Books.FindAsync(id);
            if (book == null) throw new KeyNotFoundException($"Book with ID {id} not found");

            book.Author = updateBook.Author;
            book.Cost = updateBook.Cost;

            await _dbContext.SaveChangesAsync();
            return book;
        }
    }
}