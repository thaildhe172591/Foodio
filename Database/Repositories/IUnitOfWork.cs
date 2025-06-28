using FoodioAPI.Entities.Abstractions;

namespace FoodioAPI.Database.Repositories;
public interface IUnitOfWork
{
    IBaseRepository<T> Repository<T>() where T : Entity;
    IOrderSessionRepository OrderSessionRepo { get; }
    Task<int> Save(CancellationToken cancellationToken);
    Task Rollback();
}