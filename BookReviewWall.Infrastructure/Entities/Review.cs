using System.ComponentModel.DataAnnotations;

namespace BookReviewWall.Infrastructure.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string ReviewerName { get; set; } = string.Empty;
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        [MaxLength(500)]
        public string? Comment { get; set; }
        public DateTime DateReviewed { get; set; }
        // Navigation property
        public virtual Book? Book { get; set; }
    }
}