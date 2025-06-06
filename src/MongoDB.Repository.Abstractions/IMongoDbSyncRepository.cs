﻿using MongoDB.Driver;
using MongoDB.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MongoDB.Repository
{
    public interface IMongoDbSyncRepository : IMongoDbRepository, IDisposable
    { }

    public interface ISyncMongoDbRepository<T> : IMongoDbSyncRepository, IMongoDbQueryFactory<T>, IDisposable where T : class
    {
        IList<T> Search(IMongoDbQuery<T> query);
        IList<TResult> Search<TResult>(IMongoDbQuery<T, TResult> query);
        T SingleOrDefault(IMongoDbQuery<T> query);
        TResult SingleOrDefault<TResult>(IMongoDbQuery<T, TResult> query);
        T FirstOrDefault(IMongoDbQuery<T> query);
        TResult FirstOrDefault<TResult>(IMongoDbQuery<T, TResult> query);
        T LastOrDefault(IMongoDbQuery<T> query);
        TResult LastOrDefault<TResult>(IMongoDbQuery<T, TResult> query);
        bool Any(Expression<Func<T, bool>> predicate = null);
        int Count(Expression<Func<T, bool>> predicate = null);
        long LongCount(Expression<Func<T, bool>> predicate = null);
        TResult Max<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null);
        TResult Min<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null);
        decimal Average(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate = null);
        decimal Sum(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate = null);
        object InsertOne(T entity, InsertOneOptions options = null);
        object InsertMany(IEnumerable<T> entities, InsertManyOptions options = null);
        object UpdateOne(
            Expression<Func<T, bool>> predicate,
            T entity,
            Expression<Func<T, object>>[] properties,
            UpdateOptions options = null);
        object UpdateOne(
            Expression<Func<T, bool>> predicate,
            IDictionary<Expression<Func<T, object>>, object> properties,
            UpdateOptions options = null);
        object UpdateMany(
            Expression<Func<T, bool>> predicate,
            IDictionary<Expression<Func<T, object>>, object> properties,
            UpdateOptions options = null);
        object BulkWrite(IEnumerable<WriteModel<T>> requests, BulkWriteOptions options = null);
        object ReplaceOne(Expression<Func<T, bool>> predicate, T entity, ReplaceOptions options = null);
        object DeleteOne(Expression<Func<T, bool>> predicate, DeleteOptions options = null);
        object DeleteMany(Expression<Func<T, bool>> predicate, DeleteOptions options = null);
        IQueryable<T> ToQueryable(IMongoDbQuery<T> query);
        IQueryable<TResult> ToQueryable<TResult>(IMongoDbQuery<T, TResult> query);
    }
}
