using System.Linq.Expressions;
using SA.Accounting.Core.Interfaces;

namespace SA.Accounting.Core.Entities.Interfaces;

public interface IRepository<T> where T : class
{
    // Get By Id
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Get All
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(QueryOptions options, CancellationToken cancellationToken = default);

    // Distinct Column
    List<string> GetDistinct(Expression<Func<T, string>> column);

    // Find
    Task<T?> FindAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);
    Task<T?> FindAsync(Expression<Func<T, bool>> criteria, QueryOptions options, CancellationToken cancellationToken = default);
    Task<T?> FindAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task<T?> FindAsync(Expression<Func<T, bool>> criteria, QueryOptions options, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task<T?> FindAsync(Expression<Func<T, bool>> criteria, string[] includePaths, CancellationToken cancellationToken = default);
    Task<T?> FindAsync(Expression<Func<T, bool>> criteria, string[] includePaths, QueryOptions options, CancellationToken cancellationToken = default);

    // FindAll
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, QueryOptions options, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, QueryOptions options, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includePaths, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includePaths, QueryOptions options, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAllAsync(
        Expression<Func<T, bool>> criteria,
        int? skip = null,
        int? take = null,
        string? orderBy = null,
        string? direction = null,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<T>> FindAllAsync(
        Expression<Func<T, bool>> criteria,
        string[] includePaths,
        int? skip = null,
        int? take = null,
        string? orderBy = null,
        string? direction = null,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<T>> FindAllAsync(
        Expression<Func<T, bool>> criteria,
        string[] includePaths,
        QueryOptions options,
        int? skip = null,
        int? take = null,
        string? orderBy = null,
        string? direction = null,
        CancellationToken cancellationToken = default
    );

    // Add
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    // Update
    T Update(T entity);
    bool UpdateRange(IEnumerable<T> entities);

    // Delete
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);

    // Count
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(QueryOptions options, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>> criteria, QueryOptions options, CancellationToken cancellationToken = default);

    // Max
    Task<long> MaxAsync(Expression<Func<T, object>> column, CancellationToken cancellationToken = default);
    Task<long> MaxAsync(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> column, CancellationToken cancellationToken = default);

    // Exist
    bool IsExist(Expression<Func<T, bool>> criteria);

    // Last
    T? Last(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy);
}
