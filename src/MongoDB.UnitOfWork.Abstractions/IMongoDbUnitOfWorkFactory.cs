using MongoDB.Infrastructure;

namespace MongoDB.UnitOfWork
{
    public interface IMongoDbUnitOfWorkFactory<T> where T : class, IMongoDbContext
    {
        IMongoDbUnitOfWork<T> Create(string dbContextId);
    }
}
