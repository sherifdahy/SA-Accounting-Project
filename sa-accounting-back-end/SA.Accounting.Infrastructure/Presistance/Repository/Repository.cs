using SA.Accounting.Core.Interfaces;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace SA.Accounting.Infrastructure.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    // GetById
    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<T>().FindAsync(id, cancellationToken);

    // GetAll
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Set<T>().ToListAsync(cancellationToken);

    public async Task<IEnumerable<T>> GetAllAsync(QueryOptions options, CancellationToken cancellationToken = default)
        => await ApplyOptions(_context.Set<T>(), options).ToListAsync(cancellationToken);

    // Distinct Column
    public List<string> GetDistinct(Expression<Func<T, string>> column)
        => _context.Set<T>().Select(column).Distinct().ToList();

    // Find
    public async Task<T?> FindAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default)
        => await _context.Set<T>().SingleOrDefaultAsync(criteria, cancellationToken);

    public async Task<T?> FindAsync(
        Expression<Func<T, bool>> criteria,
        QueryOptions options,
        CancellationToken cancellationToken = default)
        => await ApplyOptions(_context.Set<T>(), options).SingleOrDefaultAsync(criteria, cancellationToken);

    public async Task<T?> FindAsync(
        Expression<Func<T, bool>> criteria,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyIncludes(_context.Set<T>(), includes);

        return await query.SingleOrDefaultAsync(criteria, cancellationToken);
    }

    public async Task<T?> FindAsync(
        Expression<Func<T, bool>> criteria,
        QueryOptions options,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyOptions(_context.Set<T>(), options);
        query = ApplyIncludes(query, includes);

        return await query.SingleOrDefaultAsync(criteria, cancellationToken);
    }

    public async Task<T?> FindAsync(
        Expression<Func<T, bool>> criteria,
        string[] includePaths,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyIncludePaths(_context.Set<T>(), includePaths);

        return await query.SingleOrDefaultAsync(criteria, cancellationToken);
    }

    public async Task<T?> FindAsync(
        Expression<Func<T, bool>> criteria,
        string[] includePaths,
        QueryOptions options,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyOptions(_context.Set<T>(), options);
        query = ApplyIncludePaths(query, includePaths);

        return await query.SingleOrDefaultAsync(criteria, cancellationToken);
    }

    // FindAll
    public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default)
        => await _context.Set<T>().Where(criteria).ToListAsync(cancellationToken);

    public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, QueryOptions options, CancellationToken cancellationToken = default)
        => await ApplyOptions(_context.Set<T>(), options).Where(criteria).ToListAsync(cancellationToken);

    public async Task<IEnumerable<T>> FindAllAsync(
        Expression<Func<T, bool>> criteria,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyIncludes(_context.Set<T>(), includes);

        return await query.Where(criteria).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAllAsync(
        Expression<Func<T, bool>> criteria,
        QueryOptions options,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        var query = ApplyOptions(_context.Set<T>(), options);
        query = ApplyIncludes(query, includes);

        return await query.Where(criteria).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAllAsync(
        Expression<Func<T, bool>> criteria,
        string[] includePaths,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyIncludePaths(_context.Set<T>(), includePaths);

        return await query.Where(criteria).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAllAsync(
        Expression<Func<T, bool>> criteria,
        string[] includePaths,
        QueryOptions options,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyOptions(_context.Set<T>(), options);
        query = ApplyIncludePaths(query, includePaths);

        return await query.Where(criteria).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAllAsync(
        Expression<Func<T, bool>> criteria,
        int? skip = null,
        int? take = null,
        string? orderBy = null,
        string? direction = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyPagingAndSorting(_context.Set<T>().Where(criteria), skip, take, orderBy, direction);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAllAsync(
        Expression<Func<T, bool>> criteria,
        string[] includePaths,
        int? skip = null,
        int? take = null,
        string? orderBy = null,
        string? direction = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyIncludePaths(_context.Set<T>(), includePaths).Where(criteria);
        query = ApplyPagingAndSorting(query, skip, take, orderBy, direction);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindAllAsync(
        Expression<Func<T, bool>> criteria,
        string[] includePaths,
        QueryOptions options,
        int? skip = null,
        int? take = null,
        string? orderBy = null,
        string? direction = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyOptions(_context.Set<T>(), options);
        query = ApplyIncludePaths(query, includePaths).Where(criteria);
        query = ApplyPagingAndSorting(query, skip, take, orderBy, direction);

        return await query.ToListAsync(cancellationToken);
    }

    // Add
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    // Update
    public T Update(T entity)
    {
        _context.Update(entity);
        return entity;
    }

    public bool UpdateRange(IEnumerable<T> entities)
    {
        _context.UpdateRange(entities);
        return true;
    }

    // Delete
    public void Delete(T entity) => _context.Set<T>().Remove(entity);

    public void DeleteRange(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);

    // Count
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Set<T>().CountAsync(cancellationToken);

    public async Task<int> CountAsync(QueryOptions options, CancellationToken cancellationToken = default)
        => await ApplyOptions(_context.Set<T>(), options).CountAsync(cancellationToken);

    public async Task<int> CountAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default)
        => await _context.Set<T>().CountAsync(criteria, cancellationToken);

    public async Task<int> CountAsync(Expression<Func<T, bool>> criteria, QueryOptions options, CancellationToken cancellationToken = default)
        => await ApplyOptions(_context.Set<T>(), options).CountAsync(criteria, cancellationToken);

    // Max
    public async Task<long> MaxAsync(Expression<Func<T, object>> column, CancellationToken cancellationToken = default)
        => Convert.ToInt64(await _context.Set<T>().MaxAsync(column, cancellationToken));

    public async Task<long> MaxAsync(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> column, CancellationToken cancellationToken = default)
        => Convert.ToInt64(await _context.Set<T>().Where(criteria).MaxAsync(column, cancellationToken));

    // Exist
    public bool IsExist(Expression<Func<T, bool>> criteria)
        => _context.Set<T>().Any(criteria);

    // Last
    public T? Last(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy)
    {
        return _context.Set<T>()
            .Where(criteria)
            .OrderByDescending(orderBy)
            .FirstOrDefault();
    }

    private static IQueryable<T> ApplyIncludes(IQueryable<T> query, IEnumerable<Expression<Func<T, object>>> includes)
    {
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }

    private static IQueryable<T> ApplyOptions(IQueryable<T> query, QueryOptions options)
    {
        if (options.IncludeDeleted)
            query = query.IgnoreQueryFilters();

        if (options.AsNoTracking)
            query = query.AsNoTracking();

        return query;
    }

    private static IQueryable<T> ApplyIncludePaths(IQueryable<T> query, IEnumerable<string> includePaths)
    {
        foreach (var includePath in includePaths.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            query = query.Include(includePath);
        }

        return query;
    }

    private static IQueryable<T> ApplyPagingAndSorting(
        IQueryable<T> query,
        int? skip,
        int? take,
        string? orderBy,
        string? direction)
    {
        if (orderBy != null && direction != null)
            query = query.OrderBy($"{orderBy} {direction}");

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (take.HasValue)
            query = query.Take(take.Value);

        return query;
    }
}
