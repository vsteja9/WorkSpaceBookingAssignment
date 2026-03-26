using BooksDtos;
using WorkSpaceBookingAssignment.Repository;

namespace WorkSpaceBookingAssignment.Services
{
    /// <summary>
    /// Interface for Books Service - follow dependency inversion principle
    /// </summary>
    public interface IBooksService
    {
        Task<BookResponse> CreateBookAsync(CreateBookRequest createBook);
        Task<BookResponse> GetBookByIdAsync(Guid id);
        Task<List<BookResponse>> GetBooksAsync();
        Task<BookResponse> UpdateBookAsync(Guid id, updateBook updateBook);
        Task<bool> DeleteBookAsync(Guid id);
    }

    public class BooksService : IBooksService
    {
        private readonly IBooksRepository _booksRepository;

        public BooksService(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }

        public async Task<BookResponse> CreateBookAsync(CreateBookRequest createBook)
        {
            var book = await _booksRepository.createBook(createBook);
            return BookResponse.FromBook(book);
        }

        public async Task<List<BookResponse>> GetBooksAsync()
        {
            var books = await _booksRepository.getBooks();
            return books.Select(BookResponse.FromBook).ToList();
        }

        public async Task<BookResponse> GetBookByIdAsync(Guid id)
        {
            var book = await _booksRepository.getBookById(id);
            return BookResponse.FromBook(book);
        }

        public async Task<BookResponse> UpdateBookAsync(Guid id, updateBook updateBook)
        {
            var book = await _booksRepository.updateBook(id, updateBook);
            return BookResponse.FromBook(book);
        }

        public async Task<bool> DeleteBookAsync(Guid id)
        {
            return await _booksRepository.deleteBook(id);
        }
    }
}