using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace TaskManager.Application.Common.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // القراءة (Read)
        Task<List<T>> GetAll(CancellationToken cancellationToken = default);
        Task<List<T>> GetAll(Expression<Func<T, object>> Include, CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<T?> GetById(long id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include, CancellationToken cancellationToken = default);
        Task<T> GetFirstAsync(CancellationToken cancellationToken = default);
        Task<T?> GetByCriteria(Expression<Func<T, bool>> Criteria, CancellationToken cancellationToken = default);
        Task<T?> GetByCriteria(Expression<Func<T, bool>> Criteria, Expression<Func<T, object>> Include, CancellationToken cancellationToken = default);
        Task<List<T>> GetListByCriteria(Expression<Func<T, bool>> Criteria, CancellationToken cancellationToken = default);
        Task<List<T>> GetListByCriteria(Expression<Func<T, bool>> Criteria, Expression<Func<T, object>> Include, CancellationToken cancellationToken = default);

        // التحقق والـ Query
        Task<bool> IsExist(Expression<Func<T, bool>> Criteria, CancellationToken cancellationToken = default);
        Task<bool> Any(CancellationToken cancellationToken = default);
        IQueryable<T> FromSqlRaw(string sql);
        IQueryable<T> AsQueryable();
        ChangeTracker ChangeTracker(CancellationToken cancellationToken = default);

        // العمليات (CUD)
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(T entity, CancellationToken ct = default);
    }
}