using BookReviewWall.Infrastructure.Entities;

namespace BookReviewWall_BSIT31A3_Vernalyn_Cenardo.Models
{
    public class BookDetailsViewModel
    {
        public Book Book { get; set; } = new Book();
        public AddReviewViewModel NewReview { get; set; } = new AddReviewViewModel();
        public bool HasUserReviewed { get; set; }
        public bool IsOwner { get; set; }
    }
}