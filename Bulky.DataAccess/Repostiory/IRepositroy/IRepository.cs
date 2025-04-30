using System.Linq.Expressions;
namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // Get all entities asynchronously
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        // Get a single entity asynchronously with filtering
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties,bool tracked = false);

        // Add an entity asynchronously
        Task AddAsync(T entity);

        // Remove an entity
        void Remove(T entity);

        // Remove multiple entities
        void RemoveRange(IEnumerable<T> entities);
    }
}
