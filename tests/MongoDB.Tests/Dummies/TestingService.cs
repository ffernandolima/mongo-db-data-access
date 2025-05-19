using Microsoft.Extensions.DependencyInjection;
using MongoDB.UnitOfWork;

namespace MongoDB.Tests.Dummies
{
    public interface ITestingService
    {
        IMongoDbUnitOfWork<TestingContext> UnitOfWork { get; }
    }

    public class TestingService : ITestingService
    {
        public IMongoDbUnitOfWork<TestingContext> UnitOfWork { get; }

        public TestingService([FromKeyedServices("TestingContext - 1")] IMongoDbUnitOfWork<TestingContext> unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}
