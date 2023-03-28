using System;
using System.Linq;

namespace MongoDB.QueryBuilder
{
    public interface IMongoDbSorting<T> : IMongoDbSorting
    {
        Func<IQueryable<T>, IOrderedQueryable<T>> KeySelector { get; }
    }

    public interface IMongoDbSorting
    {
        string FieldName { get; }
        MongoDbSortingDirection SortingDirection { get; }
    }
}
