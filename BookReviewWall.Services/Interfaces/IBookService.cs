using BookReviewWall.Infrastructure.Entities;

namespace BookReviewWall.Services.Interfaces
{
    public interface IBookService
    {
        // Book operations
        Task<List<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task<bool> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<List<Book>> GetBooksByUserIdAsync(string userId);

        // Review operations
        Task<Review> AddReviewAsync(Review review);
        Task<List<Review>> GetReviewsByBookIdAsync(int bookId);
        Task<double> GetAverageRatingAsync(int bookId);
        Task<bool> HasUserReviewedBookAsync(int bookId, string userId);
    }
}