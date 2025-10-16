using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SGE.Application.Interfaces.Repositories;
using SGE.Infrastructure.Data;

namespace SGE.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken) != null;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync([id], cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}