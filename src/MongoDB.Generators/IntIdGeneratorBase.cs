using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

namespace MongoDB.Generators
{
    /// <summary>
    /// Base class for id generator based on integer values.
    /// </summary>
    public abstract class IntIdGeneratorBase<T> : IIdGenerator
    {
        #region Fields

        private readonly string _idCollectionName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDB.Generators.IntIdGeneratorBase{T}"/> class.
        /// </summary>
        /// <param name="idCollectionName">Identifier collection name.</param>
        protected IntIdGeneratorBase(string idCollectionName) => _idCollectionName = idCollectionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDB.Generators.IntIdGeneratorBase{T}"/> class.
        /// </summary>
        protected IntIdGeneratorBase()
            : this("Ids")
        { }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates the update definition.
        /// </summary>
        /// <returns>The update definition.</returns>
        protected abstract UpdateDefinition<BsonDocument> CreateUpdateDefinition();

        /// <summary>
        /// Converts to the new data type.
        /// </summary>
        /// <returns>The converted data.</returns>
        /// <param name="value">Value.</param>
        protected abstract object Convert(BsonValue value);

        /// <summary>
        /// Tests whether an Id is empty.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns>True if the Id is empty.</returns>
        public abstract bool IsEmpty(object id);

        /// <summary>
        /// Generates an Id for a document.
        /// </summary>
        /// <param name="container">The container of the document (it will be an <see cref="MongoDB.Driver.IMongoCollection{T}"/> when called from the C# driver).</param>
        /// <param name="document">The document.</param>
        /// <returns>An Id.</returns>
        public object GenerateId(object container, object document)
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var collection = (IMongoCollection<T>)container;

            var idSequenceCollection = collection.Database.GetCollection<BsonDocument>(_idCollectionName);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", collection.CollectionNamespace.CollectionName);

            var definition = CreateUpdateDefinition();

            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var documentResult = idSequenceCollection.FindOneAndUpdate(filter, definition, options);

            var documentValue = documentResult["seq"];

            var convertedValue = Convert(documentValue);

            return convertedValue;
        }

        #endregion Methods
    }
}

