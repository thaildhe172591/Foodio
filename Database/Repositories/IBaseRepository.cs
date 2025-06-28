using System.Linq.Expressions;
using FoodioAPI.Entities.Abstractions;

namespace FoodioAPI.Database.Repositories;

public interface IBaseRepository<T> where T : class, IEntity
{
    IQueryable<T> Entities { get; }
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangeAsync();
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? include = null);

}