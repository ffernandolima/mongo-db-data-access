using Microsoft.Extensions.DependencyInjection;
using MongoDB.Data;
using MongoDB.Models;
using MongoDB.Tests.Fixtures;
using MongoDB.Tests.Infrastructure;
using MongoDB.UnitOfWork;
using System.Threading.Tasks;
using Xunit;

namespace MongoDB.Tests.Implementation
{
    public class AsyncDataAccessTests : InfrastructureTestsBase
    {
        private readonly IMongoDbUnitOfWork _unitOfWork;
        private readonly IMongoDbUnitOfWork<BloggingContext> _unitOfWorkOfT;

        private readonly IMongoDbRepositoryFactory _repositoryFactory;
        private readonly IMongoDbRepositoryFactory<BloggingContext> _repositoryFactoryOfT;

        public AsyncDataAccessTests(InfrastructureFixture infrastructureFixture)
            : base(infrastructureFixture)
        {
            // IUnitOfWork used for reading/writing scenario;
            _unitOfWork = ServiceProvider.GetRequiredService<IMongoDbUnitOfWork>();
            // IUnitOfWork<T> used for used for multiple databases scenario;
            _unitOfWorkOfT = ServiceProvider.GetRequiredService<IMongoDbUnitOfWork<BloggingContext>>();

            // IRepositoryFactory used for readonly scenario;
            _repositoryFactory = ServiceProvider.GetRequiredService<IMongoDbRepositoryFactory>();
            // IRepositoryFactory<T> used for readonly/multiple databases scenario;
            _repositoryFactoryOfT = ServiceProvider.GetRequiredService<IMongoDbRepositoryFactory<BloggingContext>>();
        }

        [Fact]
        public async Task GetBlogCountAsync()
        {
            var repository = _unitOfWork.Repository<Blog>();

            var count = await repository.CountAsync().ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(50, count);

            var longCount = await repository.LongCountAsync().ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(50, longCount);
        }

        [Fact]
        public async Task AddUnsuccessfulBlogWithinTransactionAsync()
        {
            var repository = _unitOfWork.Repository<Blog>();

            var blog = Seeder.SeedBlog(51);

            _unitOfWork.StartTransaction();

            var insertOneResult = await repository.InsertOneAsync(blog).ConfigureAwait(continueOnCapturedContext: false);

            var saveChangesResult = await _unitOfWork.SaveChangesAsync().ConfigureAwait(continueOnCapturedContext: false);

            await _unitOfWork.AbortTransactionAsync().ConfigureAwait(continueOnCapturedContext: false);

            var id = await repository.MaxAsync(x => x.Id).ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(50, id);
        }
    }
}
