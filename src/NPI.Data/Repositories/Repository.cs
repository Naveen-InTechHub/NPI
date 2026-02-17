using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NPI.Data.Context;
using NPI.Data.Entities;

namespace NPI.Data.Repositories;

// ── Generic Repository Interface ──
public interface IRepository<T> where T : BaseEntity
{
    // Queries (LINQ)
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[] includes);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    // Paged queries
    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[] includes);

    // Commands
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}

// ── Generic Repository Implementation ──
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly NpiDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(NpiDbContext context) 
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // ── Queries ──

    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
            query = query.Include(include);
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
        => await _dbSet.AsNoTracking().ToListAsync();

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.AsNoTracking().Where(predicate).ToListAsync();

    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        foreach (var include in includes)
            query = query.Include(include);

        if (orderBy != null)
            query = orderBy(query);

        return await query.ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.AnyAsync(predicate);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        => predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);

    // ── Paged ──

    public async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        foreach (var include in includes)
            query = query.Include(include);

        var totalCount = await query.CountAsync();

        if (orderBy != null)
            query = orderBy(query);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    // ── Commands ──

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
        => await _dbSet.AddRangeAsync(entities);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Remove(T entity)
        => _dbSet.Remove(entity);

    public void RemoveRange(IEnumerable<T> entities)
        => _dbSet.RemoveRange(entities);
}
