using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace MongoDB.Generators
{
    /// <summary>
    /// Int32 identifier generator.
    /// </summary>
    public class Int32IdGenerator<T> : IntIdGeneratorBase<T>
    {
        private static readonly Lazy<Int32IdGenerator<T>> GeneratorFactory = new(() => new Int32IdGenerator<T>(), isThreadSafe: true);

        public static Int32IdGenerator<T> Instance => GeneratorFactory.Value;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32IdGenerator"/> class.
        /// </summary>
        /// <param name="idCollectionName">Identifier collection name.</param>
        public Int32IdGenerator(string idCollectionName)
            : base(idCollectionName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32IdGenerator"/> class.
        /// </summary>
        public Int32IdGenerator()
            : base("IdInt32")
        { }

        #endregion Constructors

        #region IntIdGeneratorBase Members

        /// <summary>
        /// Creates the update definition.
        /// </summary>
        /// <returns>The update definition.</returns>
        protected override UpdateDefinition<BsonDocument> CreateUpdateDefinition() => Builders<BsonDocument>.Update.Inc(x => x["seq"], 1);

        /// <summary>
        /// Converts to the new data type.
        /// </summary>
        /// <returns>The converted data.</returns>
        /// <param name="value">Value.</param>
        protected override object Convert(BsonValue value) => value.AsInt32;

        /// <summary>
        /// Tests whether an Id is empty.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns>True if the Id is empty.</returns>
        public override bool IsEmpty(object id) => (int)id == 0;

        #endregion IntIdGeneratorBase Members
    }
}
