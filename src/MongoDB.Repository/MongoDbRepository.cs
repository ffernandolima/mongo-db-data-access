using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Infrastructure;
using MongoDB.QueryBuilder;
using MongoDB.Repository.Extensions;
using MongoDB.Repository.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Repository
{
    public class MongoDbRepository<T> : IMongoDbRepository<T> where T : class
    {
        #region Protected Properties

        protected IMongoDbContext Context { get; }
        protected IMongoCollection<T> Collection { get; }

        #endregion Protected Properties

        #region Ctor

        public MongoDbRepository(IMongoDbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context), $"{nameof(context)} cannot be null.");
            Collection = context.GetCollection<T>();
        }

        #endregion Ctor

        #region IQueryFactory<T> Members

        public virtual IMongoDbSingleResultQuery<T> SingleResultQuery() => MongoDbSingleResultQuery<T>.New();
        public virtual IMongoDbMultipleResultQuery<T> MultipleResultQuery() => MongoDbMultipleResultQuery<T>.New();

        public virtual IMongoDbSingleResultQuery<T, TResult> SingleResultQuery<TResult>() => MongoDbSingleResultQuery<T, TResult>.New();
        public virtual IMongoDbMultipleResultQuery<T, TResult> MultipleResultQuery<TResult>() => MongoDbMultipleResultQuery<T, TResult>.New();

        #endregion IQueryFactory<T> Members

        #region ISyncRepository<T> Members

        public virtual IList<T> Search(IMongoDbQuery<T> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entities = queryable.ToList();

            return entities;
        }

        public virtual IList<TResult> Search<TResult>(IMongoDbQuery<T, TResult> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            if (query.Selector == null)
            {
                throw new ArgumentNullException(nameof(query.Selector), $"{nameof(query.Selector)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entities = queryable.ToList();

            return entities;
        }

        public virtual T SingleOrDefault(IMongoDbQuery<T> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entity = queryable.SingleOrDefault();

            return entity;
        }

        public virtual TResult SingleOrDefault<TResult>(IMongoDbQuery<T, TResult> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            if (query.Selector == null)
            {
                throw new ArgumentNullException(nameof(query.Selector), $"{nameof(query.Selector)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entity = queryable.SingleOrDefault();

            return entity;
        }

        public virtual T FirstOrDefault(IMongoDbQuery<T> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entity = queryable.FirstOrDefault();

            return entity;
        }

        public virtual TResult FirstOrDefault<TResult>(IMongoDbQuery<T, TResult> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            if (query.Selector == null)
            {
                throw new ArgumentNullException(nameof(query.Selector), $"{nameof(query.Selector)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entity = queryable.FirstOrDefault();

            return entity;
        }

        public virtual T LastOrDefault(IMongoDbQuery<T> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entity = queryable.LastOrDefault();

            return entity;
        }

        public virtual TResult LastOrDefault<TResult>(IMongoDbQuery<T, TResult> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            if (query.Selector == null)
            {
                throw new ArgumentNullException(nameof(query.Selector), $"{nameof(query.Selector)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entity = queryable.LastOrDefault();

            return entity;
        }

        public virtual bool Any(Expression<Func<T, bool>> predicate = null)
        {
            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.Any() : queryable.Any(predicate);

            return result;
        }

        public virtual int Count(Expression<Func<T, bool>> predicate = null)
        {
            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.Count() : queryable.Count(predicate);

            return result;
        }

        public virtual long LongCount(Expression<Func<T, bool>> predicate = null)
        {
            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.LongCount() : queryable.LongCount(predicate);

            return result;
        }

        public virtual TResult Max<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector), $"{nameof(selector)} cannot be null.");
            }

            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.Max(selector) : queryable.Where(predicate).Max(selector);

            return result;
        }

        public virtual TResult Min<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector), $"{nameof(selector)} cannot be null.");
            }

            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.Min(selector) : queryable.Where(predicate).Min(selector);

            return result;
        }

        public virtual decimal Average(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate = null)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector), $"{nameof(selector)} cannot be null.");
            }

            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.Average(selector) : queryable.Where(predicate).Average(selector);

            return result;
        }

        public virtual decimal Sum(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate = null)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector), $"{nameof(selector)} cannot be null.");
            }

            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.Sum(selector) : queryable.Where(predicate).Sum(selector);

            return result;
        }

        public virtual object InsertOne(T entity, InsertOneOptions options = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"{nameof(entity)} cannot be null.");
            }

            bool DoInsertOne()
            {
                if (Context.Session != null)
                {
                    Collection.InsertOne(Context.Session, entity, options);
                }
                else
                {
                    Collection.InsertOne(entity, options);
                }

                return true;
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommand(() => DoInsertOne());
            }
            else
            {
                return DoInsertOne();
            }
        }

        public virtual object InsertMany(IEnumerable<T> entities, InsertManyOptions options = null)
        {
            if (entities == null || !entities.Any())
            {
                throw new ArgumentException($"{nameof(entities)} cannot be null or empty.", nameof(entities));
            }

            bool DoInsertMany()
            {
                if (Context.Session != null)
                {
                    Collection.InsertMany(Context.Session, entities, options);
                }
                else
                {
                    Collection.InsertMany(entities, options);
                }

                return true;
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommand(() => DoInsertMany());
            }
            else
            {
                return DoInsertMany();
            }
        }

        public virtual object UpdateOne(Expression<Func<T, bool>> predicate, T entity, Expression<Func<T, object>>[] properties, UpdateOptions options = null)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), $"{nameof(predicate)} cannot be null.");
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"{nameof(entity)} cannot be null.");
            }

            if (properties == null || !properties.Any())
            {
                throw new ArgumentException($"{nameof(properties)} cannot be null or empty.", nameof(properties));
            }

            UpdateDefinition<T> definition = null;

            foreach (var property in properties)
            {
                var compiled = property.Compile();

                var value = compiled.Invoke(entity);

                if (definition == null)
                {
                    definition = Builders<T>.Update.Set(property, value);
                }
                else
                {
                    definition = definition.Set(property, value);
                }
            }

            UpdateResult DoUpdateOne()
            {
                UpdateResult result = null;

                if (Context.Session != null)
                {
                    result = Collection.UpdateOne(Context.Session, predicate, definition, options);
                }
                else
                {
                    result = Collection.UpdateOne(predicate, definition, options);
                }

                return result;
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommand(() => DoUpdateOne());
            }
            else
            {
                return DoUpdateOne();
            }
        }

        public virtual object ReplaceOne(Expression<Func<T, bool>> predicate, T entity, ReplaceOptions options = null)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), $"{nameof(predicate)} cannot be null.");
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"{nameof(entity)} cannot be null.");
            }

            ReplaceOneResult DoReplaceOne()
            {
                ReplaceOneResult result = null;

                if (Context.Session != null)
                {
                    result = Collection.ReplaceOne(Context.Session, predicate, entity, options);
                }
                else
                {
                    result = Collection.ReplaceOne(predicate, entity, options);
                }

                return result;
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommand(() => DoReplaceOne());
            }
            else
            {
                return DoReplaceOne();
            }
        }

        public virtual object DeleteOne(Expression<Func<T, bool>> predicate, DeleteOptions options = null)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), $"{nameof(predicate)} cannot be null.");
            }

            DeleteResult DoDeleteOne()
            {
                DeleteResult result = null;

                if (Context.Session != null)
                {
                    result = Collection.DeleteOne(Context.Session, predicate, options);
                }
                else
                {
                    result = Collection.DeleteOne(predicate, options);
                }

                return result;
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommand(() => DoDeleteOne());
            }
            else
            {
                return DoDeleteOne();
            }
        }

        public virtual object DeleteMany(Expression<Func<T, bool>> predicate, DeleteOptions options = null)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), $"{nameof(predicate)} cannot be null.");
            }

            DeleteResult DoDeleteMany()
            {
                DeleteResult result = null;

                if (Context.Session != null)
                {
                    result = Collection.DeleteMany(Context.Session, predicate, options);
                }
                else
                {
                    result = Collection.DeleteMany(predicate, options);
                }

                return result;
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommand(() => DoDeleteMany());
            }
            else
            {
                return DoDeleteMany();
            }
        }

        #endregion ISyncRepository<T> Members

        #region IAsyncRepository<T> Members

        public virtual Task<IList<T>> SearchAsync(IMongoDbQuery<T> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entities = queryable.ToListAsync(cancellationToken).Then<List<T>, IList<T>>(result => result, cancellationToken);

            return entities;
        }

        public virtual Task<IList<TResult>> SearchAsync<TResult>(IMongoDbQuery<T, TResult> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            if (query.Selector == null)
            {
                throw new ArgumentNullException(nameof(query.Selector), $"{nameof(query.Selector)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entities = queryable.ToListAsync(cancellationToken).Then<List<TResult>, IList<TResult>>(result => result, cancellationToken);

            return entities;
        }

        public virtual Task<T> SingleOrDefaultAsync(IMongoDbQuery<T> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entity = queryable.SingleOrDefaultAsync(cancellationToken);

            return entity;
        }

        public virtual Task<TResult> SingleOrDefaultAsync<TResult>(IMongoDbQuery<T, TResult> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            if (query.Selector == null)
            {
                throw new ArgumentNullException(nameof(query.Selector), $"{nameof(query.Selector)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entity = queryable.SingleOrDefaultAsync(cancellationToken);

            return entity;
        }

        public virtual Task<T> FirstOrDefaultAsync(IMongoDbQuery<T> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entity = queryable.FirstOrDefaultAsync(cancellationToken);

            return entity;
        }

        public virtual Task<TResult> FirstOrDefaultAsync<TResult>(IMongoDbQuery<T, TResult> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query), $"{nameof(query)} cannot be null.");
            }

            if (query.Selector == null)
            {
                throw new ArgumentNullException(nameof(query.Selector), $"{nameof(query.Selector)} cannot be null.");
            }

            var queryable = ToQueryable(query);

            var entity = queryable.FirstOrDefaultAsync(cancellationToken);

            return entity;
        }

        public virtual Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.AnyAsync(cancellationToken) : queryable.AnyAsync(predicate, cancellationToken);

            return result;
        }

        public virtual Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.CountAsync(cancellationToken) : queryable.CountAsync(predicate, cancellationToken);

            return result;
        }

        public virtual Task<long> LongCountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.LongCountAsync(cancellationToken) : queryable.LongCountAsync(predicate, cancellationToken);

            return result;
        }

        public virtual Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector), $"{nameof(selector)} cannot be null.");
            }

            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.MaxAsync(selector, cancellationToken) : queryable.Where(predicate).MaxAsync(selector, cancellationToken);

            return result;
        }

        public virtual Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector), $"{nameof(selector)} cannot be null.");
            }

            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.MinAsync(selector, cancellationToken) : queryable.Where(predicate).MinAsync(selector, cancellationToken);

            return result;
        }

        public virtual Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector), $"{nameof(selector)} cannot be null.");
            }

            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.AverageAsync(selector, cancellationToken) : queryable.Where(predicate).AverageAsync(selector, cancellationToken);

            return result;
        }

        public virtual Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector), $"{nameof(selector)} cannot be null.");
            }

            var queryable = Collection.AsQueryable();

            var result = predicate == null ? queryable.SumAsync(selector, cancellationToken) : queryable.Where(predicate).SumAsync(selector, cancellationToken);

            return result;
        }

        public virtual Task<object> InsertOneAsync(T entity, InsertOneOptions options = null, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"{nameof(entity)} cannot be null.");
            }

            Task<object> DoInsertOneAsync()
            {
                Task result;

                if (Context.Session != null)
                {
                    result = Collection.InsertOneAsync(Context.Session, entity, options, cancellationToken);
                }
                else
                {
                    result = Collection.InsertOneAsync(entity, options, cancellationToken);
                }

                return result.Then<object>(() => true, cancellationToken);
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommandAsync(() => DoInsertOneAsync());
            }
            else
            {
                return DoInsertOneAsync();
            }
        }

        public virtual Task<object> InsertManyAsync(IEnumerable<T> entities, InsertManyOptions options = null, CancellationToken cancellationToken = default)
        {
            if (entities == null || !entities.Any())
            {
                throw new ArgumentException($"{nameof(entities)} cannot be null or empty.", nameof(entities));
            }

            Task<object> DoInsertManyAsync()
            {
                Task result;

                if (Context.Session != null)
                {
                    result = Collection.InsertManyAsync(Context.Session, entities, options, cancellationToken);
                }
                else
                {
                    result = Collection.InsertManyAsync(entities, options, cancellationToken);
                }

                return result.Then<object>(() => true, cancellationToken);
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommandAsync(() => DoInsertManyAsync());
            }
            else
            {
                return DoInsertManyAsync();
            }
        }

        public virtual Task<object> UpdateOneAsync(Expression<Func<T, bool>> predicate, T entity, Expression<Func<T, object>>[] properties, UpdateOptions options = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), $"{nameof(predicate)} cannot be null.");
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"{nameof(entity)} cannot be null.");
            }

            if (properties == null || !properties.Any())
            {
                throw new ArgumentException($"{nameof(properties)} cannot be null or empty.", nameof(properties));
            }

            UpdateDefinition<T> definition = null;

            foreach (var property in properties)
            {
                var compiled = property.Compile();

                var value = compiled.Invoke(entity);

                if (definition == null)
                {
                    definition = Builders<T>.Update.Set(property, value);
                }
                else
                {
                    definition = definition.Set(property, value);
                }
            }

            Task<object> DoUpdateOneAsync()
            {
                Task<UpdateResult> result;

                if (Context.Session != null)
                {
                    result = Collection.UpdateOneAsync(Context.Session, predicate, definition, options, cancellationToken);
                }
                else
                {
                    result = Collection.UpdateOneAsync(predicate, definition, options, cancellationToken);
                }

                return result.Then<UpdateResult, object>(source => source, cancellationToken);
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommandAsync(() => DoUpdateOneAsync());
            }
            else
            {
                return DoUpdateOneAsync();
            }
        }

        public virtual Task<object> ReplaceOneAsync(Expression<Func<T, bool>> predicate, T entity, ReplaceOptions options = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), $"{nameof(predicate)} cannot be null.");
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"{nameof(entity)} cannot be null.");
            }

            Task<object> DoReplaceOneAsync()
            {
                Task<ReplaceOneResult> result;

                if (Context.Session != null)
                {
                    result = Collection.ReplaceOneAsync(Context.Session, predicate, entity, options, cancellationToken);
                }
                else
                {
                    result = Collection.ReplaceOneAsync(predicate, entity, options, cancellationToken);
                }

                return result.Then<ReplaceOneResult, object>(source => source, cancellationToken);
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommandAsync(() => DoReplaceOneAsync());
            }
            else
            {
                return DoReplaceOneAsync();
            }
        }

        public virtual Task<object> DeleteOneAsync(Expression<Func<T, bool>> predicate, DeleteOptions options = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), $"{nameof(predicate)} cannot be null.");
            }

            Task<object> DoDeleteOneAsync()
            {
                Task<DeleteResult> result;

                if (Context.Session != null)
                {
                    result = Collection.DeleteOneAsync(Context.Session, predicate, options, cancellationToken);
                }
                else
                {
                    result = Collection.DeleteOneAsync(predicate, options, cancellationToken);
                }

                return result.Then<DeleteResult, object>(source => source, cancellationToken);
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommandAsync(() => DoDeleteOneAsync());
            }
            else
            {
                return DoDeleteOneAsync();
            }
        }

        public virtual Task<object> DeleteManyAsync(Expression<Func<T, bool>> predicate, DeleteOptions options = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), $"{nameof(predicate)} cannot be null.");
            }

            Task<object> DoDeleteManyAsync()
            {
                Task<DeleteResult> result;

                if (Context.Session != null)
                {
                    result = Collection.DeleteManyAsync(Context.Session, predicate, options, cancellationToken);
                }
                else
                {
                    result = Collection.DeleteManyAsync(predicate, options, cancellationToken);
                }

                return result.Then<DeleteResult, object>(source => source, cancellationToken);
            }

            if (Context.AcceptAllChangesOnSave)
            {
                return Context.AddCommandAsync(() => DoDeleteManyAsync());
            }
            else
            {
                return DoDeleteManyAsync();
            }
        }

        #endregion IAsyncRepository<T> Members

        #region Private Methods

        private IMongoQueryable<T> ToQueryable(IMongoDbQuery<T> query)
        {
            IMongoDbMultipleResultQuery<T> multipleResultQuery = null;

            if (query is IMongoDbMultipleResultQuery<T>)
            {
                multipleResultQuery = (IMongoDbMultipleResultQuery<T>)query;
            }

            IQueryable<T> queryable = Collection.AsQueryable();

            if (query.Predicate != null)
            {
                queryable = queryable.Filter(query.Predicate);
            }

            if (query.Sortings.Any())
            {
                queryable = queryable.Sort(query.Sortings);
            }

            if (multipleResultQuery != null && multipleResultQuery.Topping.IsEnabled)
            {
                queryable = queryable.Top(multipleResultQuery.Topping);
            }

            if (multipleResultQuery != null && multipleResultQuery.Paging.IsEnabled)
            {
                IQueryable<T> countQueryable = Collection.AsQueryable();

                if (multipleResultQuery.Predicate != null)
                {
                    countQueryable = countQueryable.Filter(multipleResultQuery.Predicate);
                }

                if (multipleResultQuery.Paging is MongoDbPaging paging)
                {
                    paging.TotalCount = countQueryable.Count();
                }

                queryable = queryable.Page(multipleResultQuery.Paging);
            }

            if (query.Selector != null)
            {
                queryable = queryable.Select(query.Selector);
            }

            return (IMongoQueryable<T>)queryable;
        }

        private IMongoQueryable<TResult> ToQueryable<TResult>(IMongoDbQuery<T, TResult> query)
        {
            IMongoDbMultipleResultQuery<T, TResult> multipleResultQuery = null;

            if (query is IMongoDbMultipleResultQuery<T, TResult>)
            {
                multipleResultQuery = (IMongoDbMultipleResultQuery<T, TResult>)query;
            }

            IQueryable<T> queryable = Collection.AsQueryable();

            if (query.Predicate != null)
            {
                queryable = queryable.Filter(query.Predicate);
            }

            if (query.Sortings.Any())
            {
                queryable = queryable.Sort(query.Sortings);
            }

            if (multipleResultQuery != null && multipleResultQuery.Topping.IsEnabled)
            {
                queryable = queryable.Top(multipleResultQuery.Topping);
            }

            if (multipleResultQuery != null && multipleResultQuery.Paging.IsEnabled)
            {
                IQueryable<T> countQueryable = Collection.AsQueryable();

                if (multipleResultQuery.Predicate != null)
                {
                    countQueryable = countQueryable.Filter(multipleResultQuery.Predicate);
                }

                if (multipleResultQuery.Paging is MongoDbPaging paging)
                {
                    paging.TotalCount = countQueryable.Count();
                }

                queryable = queryable.Page(multipleResultQuery.Paging);
            }

            return (IMongoQueryable<TResult>)queryable.Select(query.Selector);
        }

        #endregion Private Methods

        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}
