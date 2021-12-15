using System;
using System.Linq.Expressions;

namespace MongoDB.QueryBuilder.Abstractions
{
    public interface IMongoDbSorting<T> : IMongoDbSorting
    {
        Expression<Func<T, object>> KeySelector { get; }
    }

    public interface IMongoDbSorting
    {
        string FieldName { get; }
        MongoDbSortDirection SortDirection { get; }
    }
}
