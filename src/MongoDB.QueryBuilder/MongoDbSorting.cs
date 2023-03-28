using MongoDB.QueryBuilder.Internal;
using System;
using System.Linq;

namespace MongoDB.QueryBuilder
{
    public class MongoDbSorting<T> : MongoDbSorting, IMongoDbSorting<T>
    {
        internal MongoDbSorting()
        { }

        internal MongoDbSortingType SortingType { get; set; }
        public Func<IQueryable<T>, IOrderedQueryable<T>> KeySelector { get; internal set; }
    }

    public class MongoDbSorting : IMongoDbSorting
    {
        internal MongoDbSorting()
        { }

        public string FieldName { get; internal set; }
        public MongoDbSortingDirection SortingDirection { get; internal set; }
    }
}
