using MongoDB.Infrastructure.Abstractions;
using MongoDB.Repository.Abstractions;

namespace MongoDB.UnitOfWork.Abstractions
{
    public interface IMongoDbRepositoryFactory
    {
        T CustomRepository<T>() where T : class;
        IMongoDbRepository<T> Repository<T>() where T : class;
    }

    public interface IMongoDbRepositoryFactory<T> : IMongoDbRepositoryFactory where T : IMongoDbContext
    { }
}
