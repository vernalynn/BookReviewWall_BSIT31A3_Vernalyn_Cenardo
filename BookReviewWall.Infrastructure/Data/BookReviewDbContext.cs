using BookReviewWall.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookReviewWall.Infrastructure.Data
{
    public class BookReviewDbContext : DbContext
    {
        public BookReviewDbContext(DbContextOptions<BookReviewDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Book entity
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(b => b.Author)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(b => b.Description)
                    .HasMaxLength(1000);

                entity.Property(b => b.UserId)
                    .IsRequired();

                // Configure one-to-many relationship
                entity.HasMany(b => b.Reviews)
                    .WithOne(r => r.Book)
                    .HasForeignKey(r => r.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Review entity
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.ReviewerName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(r => r.Rating)
                    .IsRequired();

                entity.Property(r => r.Comment)
                    .HasMaxLength(500);

                entity.Property(r => r.UserId)
                    .IsRequired();
            });
        }
    }
}