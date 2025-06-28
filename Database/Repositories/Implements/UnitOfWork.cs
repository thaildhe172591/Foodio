using System.Collections;
using FoodioAPI.Entities.Abstractions;

namespace FoodioAPI.Database.Repositories.Implements;

public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private Hashtable _repositories = [];
    public IOrderSessionRepository OrderSessionRepo => _orderSessionRepo ??= new OrderSessionRepository(_dbContext);
    private IOrderSessionRepository? _orderSessionRepo;
    public IBaseRepository<T> Repository<T>() where T : Entity
    {
        _repositories ??= [];
        var type = typeof(T).Name;
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(BaseRepository<>);

            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _dbContext);

            _repositories.Add(type, repositoryInstance);
}
        return (IBaseRepository<T>)_repositories[type]!;
    }

    public Task Rollback()
    {
        _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        return Task.CompletedTask;
    }

    public async Task<int> Save(CancellationToken cancellationToken)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}