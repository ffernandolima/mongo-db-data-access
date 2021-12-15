using MongoDB.QueryBuilder.Abstractions;
using System;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder
{
    public class MongoDbSorting<T> : MongoDbSorting, IMongoDbSorting<T>
    {
        internal MongoDbSorting()
        { }

        public Expression<Func<T, object>> KeySelector { get; internal set; }
    }

    public class MongoDbSorting : IMongoDbSorting
    {
        internal MongoDbSorting()
        { }

        public string FieldName { get; internal set; }
        public MongoDbSortDirection SortDirection { get; internal set; }
    }
}
