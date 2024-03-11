using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Search;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.Infrastructure.Internal
{
    internal class MongoDbThrottlingCollection<T> : IMongoCollection<T>
    {
        private readonly IMongoCollection<T> _collection;
        private readonly IMongoDbThrottlingSemaphore _semaphore;

        public MongoDbThrottlingCollection(IMongoDbThrottlingSemaphore semaphore, IMongoCollection<T> collection)
        {
            _semaphore = semaphore ?? throw new ArgumentNullException(nameof(semaphore), $"{nameof(semaphore)} cannot be null.");
            _collection = collection ?? throw new ArgumentNullException(nameof(collection), $"{nameof(collection)} cannot be null.");
        }

        public CollectionNamespace CollectionNamespace => _collection.CollectionNamespace;
        public IMongoDatabase Database => _collection.Database;
        public IBsonSerializer<T> DocumentSerializer => _collection.DocumentSerializer;
        public IMongoIndexManager<T> Indexes => _collection.Indexes;
        public IMongoSearchIndexManager SearchIndexes => _collection.SearchIndexes;
        public MongoCollectionSettings Settings => _collection.Settings;

        public IAsyncCursor<TResult> Aggregate<TResult>(
            PipelineDefinition<T, TResult> pipeline,
            AggregateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.Aggregate(pipeline, options, cancellationToken));
        }

        public IAsyncCursor<TResult> Aggregate<TResult>(
            IClientSessionHandle session,
            PipelineDefinition<T, TResult> pipeline,
            AggregateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.Aggregate(session, pipeline, options, cancellationToken));
        }

        public async Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(
            PipelineDefinition<T, TResult> pipeline,
            AggregateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.AggregateAsync(pipeline, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(
            IClientSessionHandle session,
            PipelineDefinition<T, TResult> pipeline,
            AggregateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.AggregateAsync(session, pipeline, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public void AggregateToCollection<TResult>(
            PipelineDefinition<T, TResult> pipeline,
            AggregateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            _semaphore.AddRequest(() => _collection.AggregateToCollection(pipeline, options, cancellationToken));
        }

        public void AggregateToCollection<TResult>(
            IClientSessionHandle session,
            PipelineDefinition<T, TResult> pipeline,
            AggregateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            _semaphore.AddRequest(() => _collection.AggregateToCollection(session, pipeline, options, cancellationToken));
        }

        public async Task AggregateToCollectionAsync<TResult>(
            PipelineDefinition<T, TResult> pipeline,
            AggregateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            await _semaphore.AddRequestAsync(_collection.AggregateToCollectionAsync(pipeline, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task AggregateToCollectionAsync<TResult>(
            IClientSessionHandle session,
            PipelineDefinition<T, TResult> pipeline,
            AggregateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            await _semaphore.AddRequestAsync(_collection.AggregateToCollectionAsync(session, pipeline, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public BulkWriteResult<T> BulkWrite(
            IEnumerable<WriteModel<T>> requests,
            BulkWriteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.BulkWrite(requests, options, cancellationToken));
        }

        public BulkWriteResult<T> BulkWrite(
            IClientSessionHandle session,
            IEnumerable<WriteModel<T>> requests,
            BulkWriteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.BulkWrite(session, requests, options, cancellationToken));
        }

        public async Task<BulkWriteResult<T>> BulkWriteAsync(
            IEnumerable<WriteModel<T>> requests,
            BulkWriteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.BulkWriteAsync(requests, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<BulkWriteResult<T>> BulkWriteAsync(
            IClientSessionHandle session,
            IEnumerable<WriteModel<T>> requests,
            BulkWriteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.BulkWriteAsync(session, requests, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        [Obsolete("Use CountDocuments or EstimatedDocumentCount instead.")]
        public long Count(
            FilterDefinition<T> filter,
            CountOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.Count(filter, options, cancellationToken));
        }

        [Obsolete("Use CountDocuments or EstimatedDocumentCount instead.")]
        public long Count(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            CountOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.Count(session, filter, options, cancellationToken));
        }

        [Obsolete("Use CountDocumentsAsync or EstimatedDocumentCountAsync instead.")]
        public async Task<long> CountAsync(
            FilterDefinition<T> filter,
            CountOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.CountAsync(filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        [Obsolete("Use CountDocumentsAsync or EstimatedDocumentCountAsync instead.")]
        public async Task<long> CountAsync(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            CountOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.CountAsync(session, filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public long CountDocuments(
            FilterDefinition<T> filter,
            CountOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.CountDocuments(filter, options, cancellationToken));
        }

        public long CountDocuments(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            CountOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.CountDocuments(session, filter, options, cancellationToken));
        }

        public async Task<long> CountDocumentsAsync(
            FilterDefinition<T> filter,
            CountOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.CountDocumentsAsync(filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<long> CountDocumentsAsync(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            CountOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.CountDocumentsAsync(session, filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public DeleteResult DeleteMany(
            FilterDefinition<T> filter,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.DeleteMany(filter, cancellationToken));
        }

        public DeleteResult DeleteMany(
            FilterDefinition<T> filter,
            DeleteOptions options,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.DeleteMany(filter, options, cancellationToken));
        }

        public DeleteResult DeleteMany(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            DeleteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.DeleteMany(session, filter, options, cancellationToken));
        }

        public async Task<DeleteResult> DeleteManyAsync(
            FilterDefinition<T> filter,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.DeleteManyAsync(filter, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<DeleteResult> DeleteManyAsync(
            FilterDefinition<T> filter,
            DeleteOptions options,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.DeleteManyAsync(filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<DeleteResult> DeleteManyAsync(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            DeleteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.DeleteManyAsync(session, filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public DeleteResult DeleteOne(
            FilterDefinition<T> filter,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.DeleteOne(filter, cancellationToken));
        }

        public DeleteResult DeleteOne(
            FilterDefinition<T> filter,
            DeleteOptions options,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.DeleteOne(filter, options, cancellationToken));
        }

        public DeleteResult DeleteOne(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            DeleteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.DeleteOne(session, filter, options, cancellationToken));
        }

        public async Task<DeleteResult> DeleteOneAsync(
            FilterDefinition<T> filter,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.DeleteOneAsync(filter, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<DeleteResult> DeleteOneAsync(
            FilterDefinition<T> filter,
            DeleteOptions options,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.DeleteOneAsync(filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<DeleteResult> DeleteOneAsync(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            DeleteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.DeleteOneAsync(session, filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public IAsyncCursor<TField> Distinct<TField>(
            FieldDefinition<T, TField> field,
            FilterDefinition<T> filter,
            DistinctOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.Distinct(field, filter, options, cancellationToken));
        }

        public IAsyncCursor<TField> Distinct<TField>(
            IClientSessionHandle session,
            FieldDefinition<T, TField> field,
            FilterDefinition<T> filter,
            DistinctOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.Distinct(session, field, filter, options, cancellationToken));
        }

        public async Task<IAsyncCursor<TField>> DistinctAsync<TField>(
            FieldDefinition<T, TField> field,
            FilterDefinition<T> filter,
            DistinctOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.DistinctAsync(field, filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IAsyncCursor<TField>> DistinctAsync<TField>(
            IClientSessionHandle session,
            FieldDefinition<T, TField> field,
            FilterDefinition<T> filter,
            DistinctOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.DistinctAsync(session, field, filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public IAsyncCursor<TItem> DistinctMany<TItem>(
            FieldDefinition<T, IEnumerable<TItem>> field,
            FilterDefinition<T> filter,
            DistinctOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.DistinctMany(field, filter, options, cancellationToken));
        }

        public IAsyncCursor<TItem> DistinctMany<TItem>(
            IClientSessionHandle session,
            FieldDefinition<T, IEnumerable<TItem>> field,
            FilterDefinition<T> filter,
            DistinctOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.DistinctMany(session, field, filter, options, cancellationToken));
        }

        public Task<IAsyncCursor<TItem>> DistinctManyAsync<TItem>(
            FieldDefinition<T, IEnumerable<TItem>> field,
            FilterDefinition<T> filter,
            DistinctOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.DistinctManyAsync(field, filter, options, cancellationToken));
        }

        public Task<IAsyncCursor<TItem>> DistinctManyAsync<TItem>(
            IClientSessionHandle session,
            FieldDefinition<T, IEnumerable<TItem>> field,
            FilterDefinition<T> filter,
            DistinctOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.DistinctManyAsync(session, field, filter, options, cancellationToken));
        }

        public long EstimatedDocumentCount(
            EstimatedDocumentCountOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.EstimatedDocumentCount(options, cancellationToken));
        }

        public async Task<long> EstimatedDocumentCountAsync
            (EstimatedDocumentCountOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.EstimatedDocumentCountAsync(options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(
            FilterDefinition<T> filter,
            FindOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.FindAsync(filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            FindOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.FindAsync(session, filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public TProjection FindOneAndDelete<TProjection>(
            FilterDefinition<T> filter,
            FindOneAndDeleteOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.FindOneAndDelete(filter, options, cancellationToken));
        }

        public TProjection FindOneAndDelete<TProjection>(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            FindOneAndDeleteOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.FindOneAndDelete(session, filter, options, cancellationToken));
        }

        public async Task<TProjection> FindOneAndDeleteAsync<TProjection>(
            FilterDefinition<T> filter,
            FindOneAndDeleteOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.FindOneAndDeleteAsync(filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<TProjection> FindOneAndDeleteAsync<TProjection>(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            FindOneAndDeleteOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.FindOneAndDeleteAsync(session, filter, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public TProjection FindOneAndReplace<TProjection>(
            FilterDefinition<T> filter,
            T replacement,
            FindOneAndReplaceOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.FindOneAndReplace(filter, replacement, options, cancellationToken));
        }

        public TProjection FindOneAndReplace<TProjection>(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            T replacement,
            FindOneAndReplaceOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.FindOneAndReplace(session, filter, replacement, options, cancellationToken));
        }

        public async Task<TProjection> FindOneAndReplaceAsync<TProjection>(
            FilterDefinition<T> filter,
            T replacement,
            FindOneAndReplaceOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.FindOneAndReplaceAsync(filter, replacement, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<TProjection> FindOneAndReplaceAsync<TProjection>(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            T replacement,
            FindOneAndReplaceOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.FindOneAndReplaceAsync(session, filter, replacement, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public TProjection FindOneAndUpdate<TProjection>(
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            FindOneAndUpdateOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.FindOneAndUpdate(filter, update, options, cancellationToken));
        }

        public TProjection FindOneAndUpdate<TProjection>(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            FindOneAndUpdateOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.FindOneAndUpdate(session, filter, update, options, cancellationToken));
        }

        public async Task<TProjection> FindOneAndUpdateAsync<TProjection>(
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            FindOneAndUpdateOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.FindOneAndUpdateAsync(filter, update, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<TProjection> FindOneAndUpdateAsync<TProjection>(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            FindOneAndUpdateOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.FindOneAndUpdateAsync(session, filter, update, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public IAsyncCursor<TProjection> FindSync<TProjection>(
            FilterDefinition<T> filter,
            FindOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.FindSync(filter, options, cancellationToken));
        }

        public IAsyncCursor<TProjection> FindSync<TProjection>(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            FindOptions<T, TProjection> options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.FindSync(session, filter, options, cancellationToken));
        }

        public void InsertMany(
            IEnumerable<T> documents,
            InsertManyOptions options = null,
            CancellationToken cancellationToken = default)
        {
            _semaphore.AddRequest(() => _collection.InsertMany(documents, options, cancellationToken));
        }

        public void InsertMany(
            IClientSessionHandle session,
            IEnumerable<T> documents,
            InsertManyOptions options = null,
            CancellationToken cancellationToken = default)
        {
            _semaphore.AddRequest(() => _collection.InsertMany(session, documents, options, cancellationToken));
        }

        public async Task InsertManyAsync(
            IEnumerable<T> documents,
            InsertManyOptions options = null,
            CancellationToken cancellationToken = default)
        {
            await _semaphore.AddRequestAsync(_collection.InsertManyAsync(documents, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task InsertManyAsync(
            IClientSessionHandle session,
            IEnumerable<T> documents,
            InsertManyOptions options = null,
            CancellationToken cancellationToken = default)
        {
            await _semaphore.AddRequestAsync(_collection.InsertManyAsync(session, documents, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public void InsertOne(
            T document,
            InsertOneOptions options = null,
            CancellationToken cancellationToken = default)
        {
            _semaphore.AddRequest(() => _collection.InsertOne(document, options, cancellationToken));
        }

        public void InsertOne(
            IClientSessionHandle session,
            T document,
            InsertOneOptions options = null,
            CancellationToken cancellationToken = default)
        {
            _semaphore.AddRequest(() => _collection.InsertOne(session, document, options, cancellationToken));
        }

        [Obsolete("Use the new overload of InsertOneAsync with an InsertOneOptions parameter instead.")]
        public async Task InsertOneAsync(
            T document,
            CancellationToken _cancellationToken)
        {
            await _semaphore.AddRequestAsync(_collection.InsertOneAsync(document, _cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task InsertOneAsync(
            T document,
            InsertOneOptions options = null,
            CancellationToken cancellationToken = default)
        {
            await _semaphore.AddRequestAsync(_collection.InsertOneAsync(document, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task InsertOneAsync(
            IClientSessionHandle session,
            T document,
            InsertOneOptions options = null,
            CancellationToken cancellationToken = default)
        {
            await _semaphore.AddRequestAsync(_collection.InsertOneAsync(session, document, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        [Obsolete("Use Aggregation pipeline instead.")]
        public IAsyncCursor<TResult> MapReduce<TResult>(
            BsonJavaScript map,
            BsonJavaScript reduce,
            MapReduceOptions<T, TResult> options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.MapReduce(map, reduce, options, cancellationToken));
        }

        [Obsolete("Use Aggregation pipeline instead.")]
        public IAsyncCursor<TResult> MapReduce<TResult>(
            IClientSessionHandle session,
            BsonJavaScript map,
            BsonJavaScript reduce,
            MapReduceOptions<T, TResult> options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.MapReduce(session, map, reduce, options, cancellationToken));
        }

        [Obsolete("Use Aggregation pipeline instead.")]
        public async Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(
            BsonJavaScript map,
            BsonJavaScript reduce,
            MapReduceOptions<T, TResult> options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.MapReduceAsync(map, reduce, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        [Obsolete("Use Aggregation pipeline instead.")]
        public async Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(
            IClientSessionHandle session,
            BsonJavaScript map,
            BsonJavaScript reduce,
            MapReduceOptions<T, TResult> options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.MapReduceAsync(session, map, reduce, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public IFilteredMongoCollection<TDerivedDocument> OfType<TDerivedDocument>()
            where TDerivedDocument : T
        {
            return _semaphore.AddRequest(_collection.OfType<TDerivedDocument>);
        }

        public ReplaceOneResult ReplaceOne(
            FilterDefinition<T> filter,
            T replacement,
            ReplaceOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.ReplaceOne(filter, replacement, options, cancellationToken));
        }

        [Obsolete("Use the overload that takes a ReplaceOptions instead of an UpdateOptions.")]
        public ReplaceOneResult ReplaceOne(
            FilterDefinition<T> filter,
            T replacement,
            UpdateOptions options,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.ReplaceOne(filter, replacement, options, cancellationToken));
        }

        public ReplaceOneResult ReplaceOne(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            T replacement,
            ReplaceOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.ReplaceOne(session, filter, replacement, options, cancellationToken));
        }

        [Obsolete("Use the overload that takes a ReplaceOptions instead of an UpdateOptions.")]
        public ReplaceOneResult ReplaceOne(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            T replacement,
            UpdateOptions options,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.ReplaceOne(session, filter, replacement, options, cancellationToken));
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync(
            FilterDefinition<T> filter,
            T replacement,
            ReplaceOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.ReplaceOneAsync(filter, replacement, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        [Obsolete("Use the overload that takes a ReplaceOptions instead of an UpdateOptions.")]
        public async Task<ReplaceOneResult> ReplaceOneAsync(
            FilterDefinition<T> filter,
            T replacement,
            UpdateOptions options,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.ReplaceOneAsync(filter, replacement, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            T replacement,
            ReplaceOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.ReplaceOneAsync(session, filter, replacement, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        [Obsolete("Use the overload that takes a ReplaceOptions instead of an UpdateOptions.")]
        public async Task<ReplaceOneResult> ReplaceOneAsync(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            T replacement,
            UpdateOptions options,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.ReplaceOneAsync(session, filter, replacement, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public UpdateResult UpdateMany(
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            UpdateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.UpdateMany(filter, update, options, cancellationToken));
        }

        public UpdateResult UpdateMany(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            UpdateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.UpdateMany(session, filter, update, options, cancellationToken));
        }

        public async Task<UpdateResult> UpdateManyAsync(
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            UpdateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.UpdateManyAsync(filter, update, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<UpdateResult> UpdateManyAsync(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            UpdateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.UpdateManyAsync(session, filter, update, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public UpdateResult UpdateOne(
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            UpdateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.UpdateOne(filter, update, options, cancellationToken));
        }

        public UpdateResult UpdateOne(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            UpdateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.UpdateOne(session, filter, update, options, cancellationToken));
        }

        public async Task<UpdateResult> UpdateOneAsync(
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            UpdateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.UpdateOneAsync(filter, update, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<UpdateResult> UpdateOneAsync(
            IClientSessionHandle session,
            FilterDefinition<T> filter,
            UpdateDefinition<T> update,
            UpdateOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.UpdateOneAsync(session, filter, update, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public IChangeStreamCursor<TResult> Watch<TResult>(
            PipelineDefinition<ChangeStreamDocument<T>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.Watch(pipeline, options, cancellationToken));
        }

        public IChangeStreamCursor<TResult> Watch<TResult>(
            IClientSessionHandle session,
            PipelineDefinition<ChangeStreamDocument<T>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return _semaphore.AddRequest(() => _collection.Watch(session, pipeline, options, cancellationToken));
        }

        public async Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(
            PipelineDefinition<ChangeStreamDocument<T>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.WatchAsync(pipeline, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(
            IClientSessionHandle session,
            PipelineDefinition<ChangeStreamDocument<T>, TResult> pipeline,
            ChangeStreamOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return await _semaphore.AddRequestAsync(_collection.WatchAsync(session, pipeline, options, cancellationToken))
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public IMongoCollection<T> WithReadConcern(ReadConcern readConcern)
        {
            return _collection.WithReadConcern(readConcern);
        }

        public IMongoCollection<T> WithReadPreference(ReadPreference readPreference)
        {
            return _collection.WithReadPreference(readPreference);
        }

        public IMongoCollection<T> WithWriteConcern(WriteConcern writeConcern)
        {
            return _collection.WithWriteConcern(writeConcern);
        }
    }
}
