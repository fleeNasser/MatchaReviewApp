using MatchaReviewApp.Data;
using MatchaReviewApp.Interfaces;
using MatchaReviewApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MatchaReviewApp.Repositories
{
    /// Generic repository implementation using Entity Framework.
    /// Used as a data access layer for all entities.

    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;

        // Constructor injection (Dependency Injection)
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<T> AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
