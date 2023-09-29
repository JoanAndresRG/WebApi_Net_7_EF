using System.Linq.Expressions;

namespace MagicVillaApi.Repository.Intefaces
{
    public interface IRepositoryService<T> where T : class
    {
        public Task<List<T>> GetEntities( Expression<Func<T,bool>>? filterExp = null);
        public Task<T> GetEntity(Expression<Func<T, bool>> filterExp = null, bool tracked = true );
        public Task CreateEntity(T entity);
        public Task DeleteEntity(T entity);
    }
}
