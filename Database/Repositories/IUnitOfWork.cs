using FoodioAPI.Entities.Abstractions;

namespace FoodioAPI.Database.Repositories;
public interface IUnitOfWork
{
    IBaseRepository<T> Repository<T>() where T : Entity;
    Task<int> Save(CancellationToken cancellationToken);
    Task Rollback();
}