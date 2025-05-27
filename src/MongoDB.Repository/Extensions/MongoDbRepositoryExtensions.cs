using MongoDB.Infrastructure;
using System;
using System.Reflection;

namespace MongoDB.Repository.Extensions
{
    public static class MongoDbRepositoryExtensions
    {
        public static IMongoDbContext GetMongoDbContext(
            this IMongoDbRepository repository,
            string propertyName = "Context",
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository), $"{nameof(repository)} cannot be null.");
            }

            var repositoryType = repository.GetType();

            var contextProperty = repositoryType.GetProperty(propertyName, bindingFlags);

            if (contextProperty is null)
            {
                throw new InvalidOperationException($"The repository does not contain a property named '{propertyName}'.");
            }

            if (contextProperty.GetValue(repository) is not IMongoDbContext context)
            {
                throw new InvalidCastException($"The '{propertyName}' property is null or is not of type '{nameof(IMongoDbContext)}'.");
            }

            return context;
        }
    }
}
