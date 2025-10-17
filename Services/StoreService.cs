using MatchaReviewApp.Interfaces;
using MatchaReviewApp.Models;
using Microsoft.Extensions.Logging;
namespace MatchaReviewApp.Services
{
    /// Store service containing business logic.
    public class StoreService : IStoreService
    {
        private readonly IRepository<Store> _storeRepository;
        private readonly ILogger<StoreService> _logger;

        // Constructor Injection (Dependency Injection)
        public StoreService(
            IRepository<Store> storeRepository,
            ILogger<StoreService> logger)
        {
            _storeRepository = storeRepository;
            _logger = logger;
        }

        public async Task<List<Store>> GetAllStoresAsync()
        {
            return await _storeRepository.GetAllAsync();
        }

        public async Task<Store> GetStoreByIdAsync(int id)
        {
            return await _storeRepository.GetByIdAsync(id);
        }

        public async Task<List<Store>> SearchStoresAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllStoresAsync();

            var stores = await _storeRepository.GetAllAsync();

            // Lambda expression for filtering
            return stores
                .Where(s =>
                    s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    s.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// Applies polymorphic sort strategy to store list.
        public List<Store> ApplySortStrategy(
            List<Store> stores,
            ISortStrategy sortStrategy)
        {
            return sortStrategy.Sort(stores).ToList();
        }

        public async Task<Store> CreateStoreAsync(Store store)
        {
            // Validation (business logic)
            if (string.IsNullOrWhiteSpace(store.Name))
                throw new ArgumentException("Store name is required");

            if (store.Rating < 0 || store.Rating > 5)
                throw new ArgumentException("Rating must be between 0 and 5");

            _logger.LogInformation($"Creating store: {store.Name}");
            return await _storeRepository.AddAsync(store);
        }

        public async Task<bool> UpdateStoreAsync(Store store)
        {
            var existing = await _storeRepository.GetByIdAsync(store.Id);
            if (existing == null) return false;

            await _storeRepository.UpdateAsync(store);
            _logger.LogInformation($"Updated store: {store.Name}");
            return true;
        }

        public async Task<bool> DeleteStoreAsync(int id)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null) return false;

            await _storeRepository.DeleteAsync(id);
            _logger.LogInformation($"Deleted store ID: {id}");
            return true;
        }
    }
}
