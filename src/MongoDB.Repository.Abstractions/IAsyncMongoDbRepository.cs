using MongoDB.Driver;
using MongoDB.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Repository
{
    public interface IAsyncMongoDbRepository : IMongoDbRepository, IDisposable
    { }

    public interface IAsyncMongoDbRepository<T> : IAsyncMongoDbRepository, IMongoDbQueryFactory<T>, IDisposable where T : class
    {
        Task<IList<T>> SearchAsync(IMongoDbQuery<T> query, CancellationToken cancellationToken = default);
        Task<IList<TResult>> SearchAsync<TResult>(IMongoDbQuery<T, TResult> query, CancellationToken cancellationToken = default);
        Task<T> SingleOrDefaultAsync(IMongoDbQuery<T> query, CancellationToken cancellationToken = default);
        Task<TResult> SingleOrDefaultAsync<TResult>(IMongoDbQuery<T, TResult> query, CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync(IMongoDbQuery<T> query, CancellationToken cancellationToken = default);
        Task<TResult> FirstOrDefaultAsync<TResult>(IMongoDbQuery<T, TResult> query, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<long> LongCountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<object> InsertOneAsync(T entity, InsertOneOptions options = null, CancellationToken cancellationToken = default);
        Task<object> InsertManyAsync(IEnumerable<T> entities, InsertManyOptions options = null, CancellationToken cancellationToken = default);
        Task<object> UpdateOneAsync(Expression<Func<T, bool>> predicate, T entity, Expression<Func<T, object>>[] properties, UpdateOptions options = null, CancellationToken cancellationToken = default);
        Task<object> ReplaceOneAsync(Expression<Func<T, bool>> predicate, T entity, ReplaceOptions options = null, CancellationToken cancellationToken = default);
        Task<object> DeleteOneAsync(Expression<Func<T, bool>> predicate, DeleteOptions options = null, CancellationToken cancellationToken = default);
        Task<object> DeleteManyAsync(Expression<Func<T, bool>> predicate, DeleteOptions options = null, CancellationToken cancellationToken = default);
    }
}
