using MatchaReviewApp.Data;
using MatchaReviewApp.Interfaces;
using MatchaReviewApp.Models;
using Microsoft.EntityFrameworkCore;
namespace MatchaReviewApp.Services
{
    /// <summary>
    /// Review service containing business logic.
    /// Demonstrates high cohesion and low coupling.
    /// </summary>
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly ApplicationDbContext _context;

        // Constructor Injection
        public ReviewService(
            IRepository<Review> reviewRepository,
            IRepository<Store> storeRepository,
            ApplicationDbContext context)
        {
            _reviewRepository = reviewRepository;
            _storeRepository = storeRepository;
            _context = context;
        }

        public async Task<List<Review>> GetReviewsByStoreAsync(int storeId)
        {
            // Use DbContext to eager-load User and Store so views can access review.User.FullName
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Store)
                .Where(r => r.StoreId == storeId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByUserAsync(string userId)
        {
            // Using DbContext directly for Include (eager loading)
            return await _context.Reviews
                .Include(r => r.Store)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            // Validation
            if (review.Rating < 1 || review.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            if (string.IsNullOrWhiteSpace(review.Comment))
                throw new ArgumentException("Review comment is required");

            // Check if user already reviewed this store
            var existingReview = await UserHasReviewedStoreAsync(review.UserId, review.StoreId);
            if (existingReview)
                throw new InvalidOperationException("You have already reviewed this store");

            var addedReview = await _reviewRepository.AddAsync(review);

            // Update store's average rating
            await UpdateStoreRatingAsync(review.StoreId);

            return addedReview;
        }

        public async Task<double> CalculateAverageRatingAsync(int storeId)
        {
            var reviews = await GetReviewsByStoreAsync(storeId);

            if (!reviews.Any()) return 0;

            // Lambda expression
            return reviews.Average(r => r.Rating);
        }

        public async Task<bool> UserHasReviewedStoreAsync(string userId, int storeId)
        {
            var reviews = await _reviewRepository.GetAllAsync();

            // Lambda expression
            return reviews.Any(r => r.UserId == userId && r.StoreId == storeId);
        }

        private async Task UpdateStoreRatingAsync(int storeId)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null) return;

            var averageRating = await CalculateAverageRatingAsync(storeId);
            store.Rating = (decimal)averageRating;

            await _storeRepository.UpdateAsync(store);
        }

        // --- New methods for edit/delete ---

        public async Task<Review> GetReviewByIdAsync(int id)
        {
            return await _reviewRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateReviewAsync(Review review)
        {
            var existing = await _reviewRepository.GetByIdAsync(review.Id);
            if (existing == null) return false;

            existing.Comment = review.Comment;
            existing.Rating = review.Rating;
            existing.CreatedAt = review.CreatedAt; // preserve original CreatedAt

            await _reviewRepository.UpdateAsync(existing);
            await UpdateStoreRatingAsync(existing.StoreId);
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var existing = await _reviewRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _reviewRepository.DeleteAsync(id);
            await UpdateStoreRatingAsync(existing.StoreId);
            return true;
        }
    }
}
