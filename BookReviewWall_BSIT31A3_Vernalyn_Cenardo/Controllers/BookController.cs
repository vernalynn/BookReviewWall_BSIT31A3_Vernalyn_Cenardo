using BookReviewWall.Infrastructure.Entities; // Core Entity namespace
using BookReviewWall.Models; // ViewModels namespace
using BookReviewWall.Services.Interfaces; // Service Interface namespace
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewWall_BSIT31A3_Vernalyn_Cenardo.Controllers
{
    [Authorize] // Require authentication for all actions
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly UserManager<IdentityUser> _userManager;

        public BookController(
            IBookService bookService,
            UserManager<IdentityUser> userManager)
        {
            _bookService = bookService;
            _userManager = userManager;
        }

        // GET: Book/Index - Display all books
        [AllowAnonymous] // Anyone can view the list
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksAsync();
            return View(books);
        }

        // GET: Book/Details/5 - Display book details and reviews
        [AllowAnonymous] // Anyone can view details
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var hasReviewed = false;
            var isOwner = false;

            if (!string.IsNullOrEmpty(userId))
            {
                hasReviewed = await _bookService.HasUserReviewedBookAsync(id, userId);
                isOwner = book.UserId == userId;
            }

            var viewModel = new BookDetailsViewModel
            {
                Book = book,
                NewReview = new AddReviewViewModel { BookId = id },
                HasUserReviewed = hasReviewed,
                IsOwner = isOwner
            };

            return View(viewModel);
        }

        // GET: Book/Create - Display add book form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create - Handle book creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddBookViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var book = new Book
            {
                Title = model.Title,
                Author = model.Author,
                Description = model.Description,
                UserId = userId
            };

            await _bookService.AddBookAsync(book);
            TempData["SuccessMessage"] = "Book added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Book/MyBooks - Display user's books only
        public async Task<IActionResult> MyBooks()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var books = await _bookService.GetBooksByUserIdAsync(userId);
            return View(books);
        }

        // POST: Book/AddReview - Handle review creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(AddReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var book = await _bookService.GetBookByIdAsync(model.BookId);
                if (book == null)
                    return NotFound();

                var userId = _userManager.GetUserId(User);
                var viewModel = new BookDetailsViewModel
                {
                    Book = book,
                    NewReview = model,
                    HasUserReviewed = await _bookService.HasUserReviewedBookAsync(model.BookId, userId ?? ""),
                    IsOwner = book.UserId == userId
                };
                // Return to the Details view with validation errors and existing data
                return View("Details", viewModel);
            }

            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            // Check if user already reviewed this book
            var alreadyReviewed = await _bookService.HasUserReviewedBookAsync(model.BookId, currentUserId);
            if (alreadyReviewed)
            {
                TempData["ErrorMessage"] = "You have already reviewed this book.";
                return RedirectToAction(nameof(Details), new { id = model.BookId });
            }

            // The Review entity does not have a ReviewerName field (it uses the IdentityUser)
            // We use the User object from the context, so we remove ReviewerName from Review entity
            // and instead rely on the IdentityUser's username/email for display in the View.
            var review = new Review
            {
                BookId = model.BookId,
                UserId = currentUserId,
                Rating = model.Rating,
                Comment = model.Comment
                // Note: ReviewerName is not part of the Review entity, only the ViewModel
            };

            await _bookService.AddReviewAsync(review);
            TempData["SuccessMessage"] = "Review added successfully!";
            return RedirectToAction(nameof(Details), new { id = model.BookId });
        }

        // POST: Book/Delete/5 - Delete a book (only owner can delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                TempData["ErrorMessage"] = "Book not found.";
                return RedirectToAction(nameof(Index));
            }

            var userId = _userManager.GetUserId(User);
            if (book.UserId != userId)
            {
                TempData["ErrorMessage"] = "You can only delete your own books.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _bookService.DeleteBookAsync(id);
            if (result)
                TempData["SuccessMessage"] = "Book deleted successfully!";
            else
                TempData["ErrorMessage"] = "Failed to delete book.";

            return RedirectToAction(nameof(Index));
        }
    }
}
