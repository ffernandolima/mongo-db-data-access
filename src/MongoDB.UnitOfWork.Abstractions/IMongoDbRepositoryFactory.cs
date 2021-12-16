using MongoDB.Infrastructure;
using MongoDB.Repository;

namespace MongoDB.UnitOfWork
{
    public interface IMongoDbRepositoryFactory
    {
        T CustomRepository<T>() where T : class;
        IMongoDbRepository<T> Repository<T>() where T : class;
    }

    public interface IMongoDbRepositoryFactory<T> : IMongoDbRepositoryFactory where T : IMongoDbContext
    { }
}
