using Microsoft.EntityFrameworkCore;
using FoodioAPI.Entities.Abstractions;
using FoodioAPI.Exceptions;
using Org.BouncyCastle.Asn1;
using System.Linq.Expressions;

namespace FoodioAPI.Database.Repositories.Implements;

public class BaseRepository<T>(ApplicationDbContext dbContext)
    : IBaseRepository<T> where T : Entity
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public IQueryable<T> Entities => _dbContext.Set<T>();

    public async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        var exist = _dbContext.Set<T>().Find(entity.Id);
        if (exist is null)
            throw new NotFoundException(nameof(T), entity.Id);
        
        _dbContext.Entry(exist).CurrentValues.SetValues(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbContext
            .Set<T>()
            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task SaveChangeAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? include = null)
    {
        IQueryable<T> query = _dbSet;
        if (!string.IsNullOrEmpty(include))
        {
            foreach (var includeProp in include.Split(','))
            {
                query = query.Include(includeProp.Trim());
            }
        }
        return await query.FirstOrDefaultAsync(predicate);
    }


}