using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDB.QueryBuilder.Internal
{
    internal static class MongoDbSortingExtensions
    {
        private static readonly MongoDbSortingType[] _sortingTypes = new[]
        {
            MongoDbSortingType.OrderBy,
            MongoDbSortingType.OrderByDescending
        };

        public static void Validate<T>(
            this IEnumerable<IMongoDbSorting<T>> sortings,
            MongoDbSortingType sortingType)
        {
            Validate(sortings.Cast<MongoDbSorting<T>>(), sortingType);
        }

        public static void Validate<T>(
            this IEnumerable<MongoDbSorting<T>> sortings,
            MongoDbSortingType sortingType)
        {
            if (sortings.Any(sorting => _sortingTypes.Contains(sorting.SortingType)))
            {
                return;
            }

            var builder = new StringBuilder();

            builder.Append($"{string.Join(" or ", _sortingTypes.Select(sortingType => $"'{sortingType}'"))}");
            builder.Append($" should be invoked prior to '{sortingType}'.");

            var message = builder.ToString();

            throw new InvalidOperationException(message);
        }
    }
}
