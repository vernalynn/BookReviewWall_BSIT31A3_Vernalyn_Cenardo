using System.ComponentModel.DataAnnotations;

namespace BookReviewWall.Infrastructure.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime DateAdded { get; set; }

        // User who added the book
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Calculated property - average rating
        public double AverageRating
        {
            get
            {
                if (Reviews == null || !Reviews.Any())
                    return 0;
                return Reviews.Average(r => r.Rating);
            }
        }

        public int ReviewCount => Reviews?.Count ?? 0;
    }
}