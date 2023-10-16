using MongoDB.Infrastructure;
using System;
using System.Linq;

namespace MongoDB.UnitOfWork
{
    public class MongoDbUnitOfWorkFactory<T> : IMongoDbUnitOfWorkFactory<T> where T : class, IMongoDbContext
    {
        private readonly IMongoDbServiceFactory _factory;

        public MongoDbUnitOfWorkFactory(IMongoDbServiceFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory), $"{nameof(factory)} cannot be null.");
        }

        public IMongoDbUnitOfWork<T> Create(string dbContextId)
        {
            if (string.IsNullOrWhiteSpace(dbContextId))
            {
                throw new ArgumentException($"{nameof(dbContextId)} cannot be null or whitespace.", nameof(dbContextId));
            }

            var contexts = _factory.GetServices<T>();

            var context = contexts?.SingleOrDefault(context => string.Equals(
                context?.Options?.DbContextId,
                dbContextId,
                StringComparison.OrdinalIgnoreCase));

            var unitOfWork = (IMongoDbUnitOfWork<T>)Activator.CreateInstance(typeof(MongoDbUnitOfWork<T>), context, _factory);

            return unitOfWork;
        }
    }
}
