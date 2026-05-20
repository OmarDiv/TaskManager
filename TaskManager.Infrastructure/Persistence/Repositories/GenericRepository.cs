using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Application.Common.Interfaces.Repositories;
using TaskManager.Infrastructure.Persistence.Context;

namespace TaskManager.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext Context;
        protected readonly DbSet<T> DbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAll(CancellationToken cancellationToken = default)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }

        public async Task<List<T>> GetAll(Expression<Func<T, object>> Include, CancellationToken cancellationToken = default)
        {
            return await DbSet.Include(Include).ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            return await DbSet.FindAsync(new object[] { id }, ct);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await DbSet.FindAsync(new object[] { id }, ct);
        }

        public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<T?> GetById(long id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = DbSet;
            if (include != null)
            {
                query = include(query);
            }

            // Standard approach to find by key when using IQueryable with includes
            var keyProperty = Context.Model.FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties[0];

            if (keyProperty == null)
            {
                throw new InvalidOperationException("Entity primary key not found.");
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, keyProperty.Name);
            var constant = Expression.Constant(id);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            return await query.FirstOrDefaultAsync(lambda, cancellationToken);
        }

        public async Task<T> GetFirstAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.FirstAsync(cancellationToken);
        }

        public async Task<T?> GetByCriteria(Expression<Func<T, bool>> Criteria, CancellationToken cancellationToken = default)
        {
            return await DbSet.FirstOrDefaultAsync(Criteria, cancellationToken);
        }

        public async Task<T?> GetByCriteria(Expression<Func<T, bool>> Criteria, Expression<Func<T, object>> Include, CancellationToken cancellationToken = default)
        {
            return await DbSet.Include(Include).FirstOrDefaultAsync(Criteria, cancellationToken);
        }

        public async Task<List<T>> GetListByCriteria(Expression<Func<T, bool>> Criteria, CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(Criteria).ToListAsync(cancellationToken);
        }

        public async Task<List<T>> GetListByCriteria(Expression<Func<T, bool>> Criteria, Expression<Func<T, object>> Include, CancellationToken cancellationToken = default)
        {
            return await DbSet.Include(Include).Where(Criteria).ToListAsync(cancellationToken);
        }

        public async Task<bool> IsExist(Expression<Func<T, bool>> Criteria, CancellationToken cancellationToken = default)
        {
            return await DbSet.AnyAsync(Criteria, cancellationToken);
        }

        public async Task<bool> Any(CancellationToken cancellationToken = default)
        {
            return await DbSet.AnyAsync(cancellationToken);
        }

        public IQueryable<T> FromSqlRaw(string sql)
        {
            return DbSet.FromSqlRaw(sql);
        }

        public IQueryable<T> AsQueryable()
        {
            return DbSet.AsQueryable();
        }

        public ChangeTracker ChangeTracker(CancellationToken cancellationToken = default)
        {
            return Context.ChangeTracker;
        }

        public async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            await DbSet.AddAsync(entity, ct);
            return entity;
        }

        public Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            DbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            DbSet.Remove(entity);
            return Task.CompletedTask;
        }
    }
}
