using MatchaReviewApp.Models;

namespace MatchaReviewApp.Interfaces
{
    /// <summary>
    /// Store service interface for business logic operations.
    /// Example of interface implementation (requirement: 2+ interfaces).
    /// </summary>
    public interface IStoreService
    {
        Task<List<Store>> GetAllStoresAsync();
        Task<Store> GetStoreByIdAsync(int id);
        Task<List<Store>> SearchStoresAsync(string searchTerm);
        List<Store> ApplySortStrategy(List<Store> stores, ISortStrategy sortStrategy);
        Task<Store> CreateStoreAsync(Store store);
        Task<bool> UpdateStoreAsync(Store store);
        Task<bool> DeleteStoreAsync(int id);
    }
}
