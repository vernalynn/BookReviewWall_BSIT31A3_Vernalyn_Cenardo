using BookReviewWall.Infrastructure.Data;
using BookReviewWall.Infrastructure.Entities;
using BookReviewWall.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookReviewWall.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly BookReviewDbContext _context;

        public BookService(BookReviewDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Reviews)
                .OrderByDescending(b => b.DateAdded)
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            book.DateAdded = DateTime.Now;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            var existingBook = await _context.Books.FindAsync(book.Id);
            if (existingBook == null)
                return false;

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Description = book.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Book>> GetBooksByUserIdAsync(string userId)
        {
            return await _context.Books
                .Include(b => b.Reviews)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.DateAdded)
                .ToListAsync();
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            review.DateReviewed = DateTime.Now;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<List<Review>> GetReviewsByBookIdAsync(int bookId)
        {
            return await _context.Reviews
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.DateReviewed)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BookId == bookId)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<bool> HasUserReviewedBookAsync(int bookId, string userId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.BookId == bookId && r.UserId == userId);
        }
    }
}