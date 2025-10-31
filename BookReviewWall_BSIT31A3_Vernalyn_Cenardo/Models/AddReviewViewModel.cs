using System.ComponentModel.DataAnnotations;

namespace BookReviewWall.Models
{
    public class AddReviewViewModel
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Your Name")]
        public string ReviewerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a rating")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        [Display(Name = "Rating (1-5 stars)")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        [Display(Name = "Your Review")]
        [DataType(DataType.MultilineText)]
        public string? Comment { get; set; }
    }
}