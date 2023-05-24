using Microsoft.Extensions.DependencyInjection;
using MongoDB.Data;
using MongoDB.Driver;
using MongoDB.Models;
using MongoDB.Tests.Dummies;
using MongoDB.Tests.Fixtures;
using MongoDB.Tests.Infrastructure;
using MongoDB.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace MongoDB.Tests.Implementation
{
    public class AsyncDataAccessTests : InfrastructureTestsBase
    {
        private readonly IMongoDbUnitOfWork _unitOfWork;
        private readonly IMongoDbUnitOfWork<BloggingContext> _unitOfWorkOfT;
        private readonly IMongoDbUnitOfWorkFactory<TestingContext> _unitOfWorkFactoryOfT;

        private readonly IMongoDbRepositoryFactory _repositoryFactory;
        private readonly IMongoDbRepositoryFactory<BloggingContext> _repositoryFactoryOfT;

        public AsyncDataAccessTests(InfrastructureFixture infrastructureFixture)
            : base(infrastructureFixture)
        {
            // IMongoDbUnitOfWork used for reading/writing scenario;
            _unitOfWork = ServiceProvider.GetRequiredService<IMongoDbUnitOfWork>();
            // IMongoDbUnitOfWork<T> used for used for multiple databases scenario;
            _unitOfWorkOfT = ServiceProvider.GetRequiredService<IMongoDbUnitOfWork<BloggingContext>>();
            // IMongoDbUnitOfWorkFactory<T> used for used for multi-tenant scenario;
            _unitOfWorkFactoryOfT = ServiceProvider.GetRequiredService<IMongoDbUnitOfWorkFactory<TestingContext>>();

            // IMongoDbRepositoryFactory used for readonly scenario;
            _repositoryFactory = ServiceProvider.GetRequiredService<IMongoDbRepositoryFactory>();
            // IMongoDbRepositoryFactory<T> used for readonly/multiple databases scenario;
            _repositoryFactoryOfT = ServiceProvider.GetRequiredService<IMongoDbRepositoryFactory<BloggingContext>>();
        }

        [Fact]
        public async Task GetBlogCountAsync()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var count = await repository.CountAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(50, count);

            var longCount = await repository.LongCountAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(50, longCount);
        }

        [Fact]
        public async Task UpdateManyBlogsAsync()
        {
            // Arrange
            var blogIds = new List<int> { 10, 11 };
            var newBlogTitle = "a-updated-title";

            // Act
            var repository = _unitOfWorkOfT.Repository<Blog>();

            await repository.UpdateManyAsync(
                blog => blogIds.Contains(blog.Id),
                new Dictionary<Expression<Func<Blog, object>>, object>
                {
                    { blog => blog.Title, newBlogTitle }
                })
                .ConfigureAwait(continueOnCapturedContext: false);

            await _unitOfWorkOfT.SaveChangesAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            // Assert
            var query = repository.MultipleResultQuery()
                .AndFilter(blog => blogIds.Contains(blog.Id));

            var blogResults = await repository.SearchAsync(query)
                .ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(blogResults.SingleOrDefault(blog => blog.Id == blogIds.First())?.Title, newBlogTitle);
            Assert.Equal(blogResults.SingleOrDefault(blog => blog.Id == blogIds.Last())?.Title, newBlogTitle);
        }

        [Fact]
        public async Task BulkWriteBlogsAsync()
        {
            // Arrange
            var blogIds = new List<int> { 12, 13 };
            var newBlogTitle = "updated-title";

            var requests = new List<WriteModel<Blog>>();

            foreach (var blogId in blogIds)
            {
                var filter = Builders<Blog>.Filter.Eq(blog => blog.Id, blogId);
                var definition = Builders<Blog>.Update.Set(blog => blog.Title, $"a{blogId}-{newBlogTitle}");

                requests.Add(new UpdateOneModel<Blog>(filter, definition));
            }

            // Act
            var repository = _unitOfWorkOfT.Repository<Blog>();

            await repository.BulkWriteAsync(requests)
                .ConfigureAwait(continueOnCapturedContext: false);

            await _unitOfWorkOfT.SaveChangesAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            // Assert
            var query = repository.MultipleResultQuery()
                .AndFilter(blog => blogIds.Contains(blog.Id));

            var blogResults = await repository.SearchAsync(query)
                .ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(blogResults.SingleOrDefault(blog => blog.Id == blogIds.First())?.Title, $"a{blogIds.First()}-{newBlogTitle}");
            Assert.Equal(blogResults.SingleOrDefault(blog => blog.Id == blogIds.Last())?.Title, $"a{blogIds.Last()}-{newBlogTitle}");
        }

        [Fact]
        public async Task AddUnsuccessfulBlogWithinTransactionAsync()
        {
            const int Id = 51;

            var repository = _unitOfWorkOfT.Repository<Blog>();

            var blog = Seeder.SeedBlog(Id);

            _unitOfWorkOfT.StartTransaction();

            var insertOneResult = await repository.InsertOneAsync(blog)
                .ConfigureAwait(continueOnCapturedContext: false);

            blog.Title += " - Updated";

            var UpdateOneResult = await repository.UpdateOneAsync(
                    x => x.Id == Id,
                    blog,
                    new Expression<Func<Blog, object>>[] { x => x.Title })
                .ConfigureAwait(continueOnCapturedContext: false);

            var saveChangesResult = await _unitOfWorkOfT.SaveChangesAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            await _unitOfWorkOfT.AbortTransactionAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            var id = await repository.MaxAsync(x => x.Id)
                .ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(50, id);
        }
    }
}
