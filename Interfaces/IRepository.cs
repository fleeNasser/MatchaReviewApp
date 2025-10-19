using MatchaReviewApp.Models;

namespace MatchaReviewApp.Interfaces
{
    /// Generic repository interface
    /// Provides contract for data access operations.
    public interface IRepository<T> where T : BaseEntity
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
