using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Data;
using MongoDB.Infrastructure;
using MongoDB.Infrastructure.Extensions;
using MongoDB.Models;
using MongoDB.Tests.Infrastructure;
using MongoDB.UnitOfWork;
using MongoDB.UnitOfWork.Abstractions.Extensions;
using System.Threading.Tasks;
using Xunit;

namespace MongoDB.Tests.Implementation
{
    public class AsyncDataAccessTests : Startup
    {
        private readonly IMongoDbUnitOfWork _unitOfWork;
        private readonly IMongoDbUnitOfWork<BloggingContext> _unitOfWorkOfT;

        private readonly IMongoDbRepositoryFactory _repositoryFactory;
        private readonly IMongoDbRepositoryFactory<BloggingContext> _repositoryFactoryOfT;

        public AsyncDataAccessTests()
            : base()
        {
            // IUnitOfWork used for reading/writing scenario;
            _unitOfWork = ServiceProvider.GetService<IMongoDbUnitOfWork>();
            // IUnitOfWork<T> used for used for multiple databases scenario;
            _unitOfWorkOfT = ServiceProvider.GetService<IMongoDbUnitOfWork<BloggingContext>>();

            // IRepositoryFactory used for readonly scenario;
            _repositoryFactory = ServiceProvider.GetService<IMongoDbRepositoryFactory>();
            // IRepositoryFactory<T> used for readonly/multiple databases scenario;
            _repositoryFactoryOfT = ServiceProvider.GetService<IMongoDbRepositoryFactory<BloggingContext>>();

            _ = SeedAsync();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddMongoDbContext<IMongoDbContext, BloggingContext>(provider =>
            {
                var connectionString = Configuration.GetValue<string>("MongoSettings:ConnectionString");
                var databaseName = Configuration.GetValue<string>("MongoSettings:DatabaseName");

                var bloggingContext = new BloggingContext(connectionString, databaseName);

                return bloggingContext;
            });

            services.AddMongoDbUnitOfWork();
            services.AddMongoDbUnitOfWork<BloggingContext>();
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

        private async Task SeedAsync()
        {
            var repository = _unitOfWork.Repository<Blog>();

            if (!repository.Any())
            {
                var blogs = Seeder.SeedBlogs();

                await repository.InsertManyAsync(blogs).ConfigureAwait(continueOnCapturedContext: false);

                await _unitOfWork.SaveChangesAsync().ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}
