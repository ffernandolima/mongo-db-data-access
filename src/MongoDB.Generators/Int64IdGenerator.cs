using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace MongoDB.Generators
{
    /// <summary>
    /// Int64 identifier generator.
    /// </summary>
    public sealed class Int64IdGenerator<T> : IntIdGeneratorBase<T>
    {
        private static readonly Lazy<Int64IdGenerator<T>> GeneratorFactory = new(() => new Int64IdGenerator<T>(), isThreadSafe: true);

        public static Int64IdGenerator<T> Instance => GeneratorFactory.Value;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDB.Generators.Int64IdGenerator{T}"/> class.
        /// </summary>
        /// <param name="idCollectionName">Identifier collection name.</param>
        public Int64IdGenerator(string idCollectionName)
            : base(idCollectionName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDB.Generators.Int64IdGenerator{T}"/> class.
        /// </summary>
        public Int64IdGenerator()
            : base("IdInt64")
        { }

        #endregion Constructors

        #region IntIdGeneratorBase Members

        /// <summary>
        /// Creates the update definition.
        /// </summary>
        /// <returns>The update definition.</returns>
        protected override UpdateDefinition<BsonDocument> CreateUpdateDefinition() => Builders<BsonDocument>.Update.Inc(x => x["seq"], 1L);

        /// <summary>
        /// Converts to the new data type.
        /// </summary>
        /// <returns>The converted data.</returns>
        /// <param name="value">Value.</param>
        protected override object Convert(BsonValue value) => value.AsInt64;

        /// <summary>
        /// Tests whether an Id is empty.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns>True if the Id is empty.</returns>
        public override bool IsEmpty(object id) => (long)id == 0;

        #endregion IntIdGeneratorBase Members
    }
}

