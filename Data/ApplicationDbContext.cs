using MatchaReviewApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
/// <summary>
/// Database context for the application.
/// Inherits from IdentityDbContext with custom User model.
/// </summary>
namespace MatchaReviewApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for application entities
        public DbSet<Store> Stores { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Store entity
            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
                entity.Property(s => s.Address).IsRequired().HasMaxLength(300);
                entity.Property(s => s.Rating).HasColumnType("decimal(3,2)");

                // One-to-many: Store -> Reviews
                entity.HasMany(s => s.Reviews)
                      .WithOne(r => r.Store)
                      .HasForeignKey(r => r.StoreId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Review entity
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Comment).HasMaxLength(1000);
                entity.Property(r => r.Rating).IsRequired();

                // Many-to-one: Review -> User
                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reviews)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure User entity (if additional configuration needed)
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(100);
                entity.Property(u => u.LastName).HasMaxLength(100);
                entity.Property(u => u.JoinDate).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
