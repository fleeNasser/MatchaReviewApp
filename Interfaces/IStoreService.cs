using MatchaReviewApp.Models;

namespace MatchaReviewApp.Interfaces
{
    /// Store service interface for business logic operations.
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
