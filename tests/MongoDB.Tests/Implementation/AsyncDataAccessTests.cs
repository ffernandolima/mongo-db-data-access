using Microsoft.Extensions.DependencyInjection;
using MongoDB.Data;
using MongoDB.Models;
using MongoDB.Tests.Fixtures;
using MongoDB.Tests.Infrastructure;
using MongoDB.UnitOfWork;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using MongoDB.Driver;
using System.Linq;

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
            var idBlog1 = 10;
            var idBlog2 = 11;
            var blogsId = new List<int> { idBlog1, idBlog2 };
            var updatedTitle = "a-updated-title";

            var filter = Builders<Blog>.Filter.In(b => b.Id, blogsId);
            var update = Builders<Blog>.Update.Set(b => b.Title, updatedTitle);

            // Act
            var repository = _unitOfWork.Repository<Blog>();

            await repository
                .UpdateManyAsync(filter, update)
                .ConfigureAwait(continueOnCapturedContext: false);

            await _unitOfWork
                .SaveChangesAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            // Assert
            var query = repository
                .MultipleResultQuery()
                .AndFilter(r => blogsId.Contains(r.Id));

            var result = await repository
                .SearchAsync(query)
                .ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(result.First(r => r.Id == idBlog1)?.Title, updatedTitle);
            Assert.Equal(result.First(r => r.Id == idBlog2)?.Title, updatedTitle);
        }

        [Fact]
        public async Task BulkWriteBlogsAsync()
        {
            // Arrange
            var idBlog1 = 12;
            var idBlog2 = 13;
            var blogsId = new List<int> { idBlog1, idBlog2 };
            var updatedTitle = "a-updated-title";

            var requests = new List<WriteModel<Blog>>();
            foreach (var blogId in blogsId)
            {
                var filter = Builders<Blog>.Filter.Eq(b => b.Id, blogId);
                var update = Builders<Blog>.Update.Set(b => b.Title, $"{updatedTitle}-{blogId}");

                requests.Add(new UpdateOneModel<Blog>(filter, update));
            }

            // Act
            var repository = _unitOfWork.Repository<Blog>();

            await repository
                .BulkWriteAsync(requests)
                .ConfigureAwait(continueOnCapturedContext: false);

            await _unitOfWork
                .SaveChangesAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            // Assert
            var query = repository
                .MultipleResultQuery()
                .AndFilter(r => blogsId.Contains(r.Id));

            var result = await repository
                .SearchAsync(query)
                .ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(result.First(r => r.Id == idBlog1)?.Title, $"{updatedTitle}-{idBlog1}");
            Assert.Equal(result.First(r => r.Id == idBlog2)?.Title, $"{updatedTitle}-{idBlog2}");
        }

        [Fact]
        public async Task AddUnsuccessfulBlogWithinTransactionAsync()
        {
            const int Id = 51;

            var repository = _unitOfWork.Repository<Blog>();

            var blog = Seeder.SeedBlog(Id);

            _unitOfWork.StartTransaction();

            var insertOneResult = await repository.InsertOneAsync(blog)
                .ConfigureAwait(continueOnCapturedContext: false);

            blog.Title += " - Updated";

            var UpdateOneResult = await repository.UpdateOneAsync(
                    x => x.Id == Id,
                    blog,
                    new Expression<Func<Blog, object>>[] { x => x.Title })
                .ConfigureAwait(continueOnCapturedContext: false);

            var saveChangesResult = await _unitOfWork.SaveChangesAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            await _unitOfWork.AbortTransactionAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            var id = await repository.MaxAsync(x => x.Id)
                .ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(50, id);
        }
    }
}
