using MagicVillaApi.Data;
using Microsoft.EntityFrameworkCore;
using MagicVillaApi.Repository.Intefaces;
using System.Linq.Expressions;

namespace MagicVillaApi.Repository.Implements
{
    public class RepositoryService<T> : IRepositoryService<T> where T : class

    {
        private readonly ApplicationDbContext _dbContext;
        internal DbSet<T> _dbSet; 
        private readonly ILogger _logger;

        public RepositoryService(ApplicationDbContext applicationDbContext, ILogger logger)
        {
            _dbContext = applicationDbContext;
            _logger = logger;
            this._dbSet = _dbContext.Set<T>();
        }

        public async Task CreateEntity(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task DeleteEntity(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<T> GetEntity(Expression<Func<T,bool>> filterExp = null , bool tracked = true)
        {
            try
            {
                IQueryable<T> query = _dbSet;
                if (!tracked) query = query.AsNoTracking();
                if (filterExp != null) query = query.Where(filterExp);
                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<List<T>> GetEntities(Expression<Func<T, bool>>? filterExp = null)
        {
            try
            {
                IQueryable<T> query = _dbSet;
                if (filterExp != null) query = query.Where(filterExp);
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}
