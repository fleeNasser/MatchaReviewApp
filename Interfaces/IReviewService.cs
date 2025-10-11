using MatchaReviewApp.Models;

namespace MatchaReviewApp.Interfaces
{
    /// <summary>
    /// Review service interface for business logic operations.
    /// Example of interface implementation (requirement: 2+ interfaces).
    /// </summary>
    public interface IReviewService
    {
        Task<List<Review>> GetReviewsByStoreAsync(int storeId);
        Task<List<Review>> GetReviewsByUserAsync(string userId);
        Task<Review> AddReviewAsync(Review review);
        Task<double> CalculateAverageRatingAsync(int storeId);
        Task<bool> UserHasReviewedStoreAsync(string userId, int storeId);
    }
}
