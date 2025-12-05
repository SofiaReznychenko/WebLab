using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly GymContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<Repository<T>> _logger;

    public Repository(GymContext context, ILogger<Repository<T>> logger)
    {
        _context = context;
        _logger = logger;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        _logger.LogInformation("Getting all entities of type {EntityType}", typeof(T).Name);
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        _logger.LogInformation("Finding entities of type {EntityType} with predicate", typeof(T).Name);
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        _logger.LogInformation("Adding new entity of type {EntityType}", typeof(T).Name);
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _logger.LogInformation("Updating entity of type {EntityType}", typeof(T).Name);
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _logger.LogInformation("Deleting entity of type {EntityType}", typeof(T).Name);
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteByIdAsync(int id)
    {
        _logger.LogInformation("Deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning("Entity of type {EntityType} with ID {Id} not found for deletion", typeof(T).Name, id);
        }
    }
}
