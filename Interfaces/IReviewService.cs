using MatchaReviewApp.Models;

namespace MatchaReviewApp.Interfaces
{
    /// <summary>
    /// Review service interface for business logic operations.
    /// </summary>
    public interface IReviewService
    {
        Task<List<Review>> GetReviewsByStoreAsync(int storeId);
        Task<List<Review>> GetReviewsByUserAsync(string userId);
        Task<Review> AddReviewAsync(Review review);
        Task<double> CalculateAverageRatingAsync(int storeId);
        Task<bool> UserHasReviewedStoreAsync(string userId, int storeId);

        // Added for edit/delete functionality
        Task<Review> GetReviewByIdAsync(int id);
        Task<bool> UpdateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(int id);
    }
}
