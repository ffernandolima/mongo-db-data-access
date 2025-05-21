using MongoDB.Infrastructure;
using System;

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

            var context = _factory.GetKeyedService<T>(dbContextId);

            var unitOfWork = (IMongoDbUnitOfWork<T>)Activator.CreateInstance(
                typeof(MongoDbUnitOfWork<T>),
                context,
                _factory);

            return unitOfWork;
        }
    }
}
