using Microsoft.Extensions.DependencyInjection;
using MongoDB.Data;
using MongoDB.Data.Repositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Models;
using MongoDB.QueryBuilder;
using MongoDB.Repository.Extensions;
using MongoDB.Tests.Dummies;
using MongoDB.Tests.Fixtures;
using MongoDB.Tests.Infrastructure;
using MongoDB.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace MongoDB.Tests.Implementation
{
    public class SyncDataAccessTests : InfrastructureTestsBase
    {
        private readonly IMongoDbUnitOfWork _unitOfWork;
        private readonly IMongoDbUnitOfWork<BloggingContext> _unitOfWorkOfT;
        private readonly IMongoDbUnitOfWorkFactory<TestingContext> _unitOfWorkFactoryOfT;

        private readonly IMongoDbRepositoryFactory _repositoryFactory;
        private readonly IMongoDbRepositoryFactory<BloggingContext> _repositoryFactoryOfT;

        public SyncDataAccessTests(InfrastructureFixture infrastructureFixture)
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

        [Theory]
        [InlineData($"{nameof(TestingContext)} - 1")]
        [InlineData($"{nameof(TestingContext)} - 2")]
        public void GetUnitOfWork(string dbContextId)
        {
            var unitOfWork = _unitOfWorkFactoryOfT.Create(dbContextId);

            Assert.IsAssignableFrom<IMongoDbUnitOfWork<TestingContext>>(unitOfWork);
            Assert.Equal(dbContextId, unitOfWork.Context.Options.DbContextId);
        }

        [Fact]
        public void GetAllBlogs()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var query = repository.MultipleResultQuery();

            var blogs = repository.Search(query);

            Assert.NotNull(blogs);
            Assert.Equal(50, blogs.Count);
        }

        [Fact]
        public void GetAllBlogsProjection()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var query = repository.MultipleResultQuery()
                                  .Select(selector => new
                                  {
                                      Name = selector.Title,
                                      Link = selector.Url,
                                      Type = selector.Type.Description
                                  });

            var blogs = repository.Search(query);

            Assert.NotNull(blogs);
            Assert.Equal(50, blogs.Count);
            Assert.Equal("a1", blogs[0].Name);
            Assert.Equal("/a/1", blogs[0].Link);
            Assert.Equal("z1", blogs[0].Type);
        }

        [Fact]
        public void GetAllOrderedBlogs()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            IMongoDbQuery<Blog> query;
            IList<Blog> blogs;

            query = repository.MultipleResultQuery()
                              .OrderByDescending("Id");

            blogs = repository.Search(query);

            Assert.NotNull(blogs);
            Assert.Equal(50, blogs.Count);
            Assert.Equal(50, blogs[0].Id);

            query = repository.MultipleResultQuery()
                              .OrderByDescending(blog => blog.Id);

            blogs = repository.Search(query);

            Assert.NotNull(blogs);
            Assert.Equal(50, blogs.Count);
            Assert.Equal(50, blogs[0].Id);
        }

        [Fact]
        public void GetTopBlogs()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var query = repository.MultipleResultQuery()
                                  .Top(10);

            var blogs = repository.Search(query);

            Assert.NotNull(blogs);
            Assert.Equal(10, blogs.Count);
        }

        [Fact]
        public void GetPagedBlogs()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var query = repository.MultipleResultQuery()
                                  .Page(1, 20);

            var blogs = repository.Search(query);

            Assert.NotNull(blogs);
            Assert.Equal(20, blogs.Count);
            Assert.Equal(1, query.Paging.PageIndex);
            Assert.Equal(20, query.Paging.PageSize);
            Assert.Equal(50, query.Paging.TotalCount);
        }

        [Fact]
        public void GetBlogsPagedList()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var query = repository.MultipleResultQuery()
                                  .Page(1, 20);

            var blogs = repository.Search(query)
                                  .ToPagedList(
                                    query.Paging.PageIndex,
                                    query.Paging.PageSize,
                                    query.Paging.TotalCount);

            Assert.NotNull(blogs);
            Assert.Equal(20, blogs.Count);
            Assert.Equal(1, blogs.PageIndex);
            Assert.Equal(20, blogs.PageSize);
            Assert.Equal(50, blogs.TotalCount);
            Assert.Equal(3, blogs.TotalPages);
            Assert.False(blogs.HasPreviousPage);
            Assert.True(blogs.HasNextPage);
        }

        [Fact]
        public void GetFilteredBlogs()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var query = repository.MultipleResultQuery()
                                  .AndFilter(blog => blog.Url.StartsWith("/a/"))
                                  .AndFilter(blog => blog.Title.StartsWith("a"))
                                  .AndFilter(blog => blog.Posts.Any());

            var blogs = repository.Search(query);

            Assert.NotNull(blogs);
            Assert.Equal(50, blogs.Count);
        }

        [Fact]
        public void GetBlogByUrl()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var query = repository.SingleResultQuery()
                                  .AndFilter(blog => blog.Url.StartsWith("/a/"))
                                  .OrderByDescending(blog => blog.Id);

            var blogResult = repository.FirstOrDefault(query);

            Assert.NotNull(blogResult);
            Assert.Equal(50, blogResult.Id);
            Assert.Equal("/a/50", blogResult.Url);
        }

        [Fact]
        public void GetBlogById()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var query = repository.SingleResultQuery()
                                  .AndFilter(blog => blog.Id == 1);

            var blogResult = repository.SingleOrDefault(query);

            Assert.NotNull(blogResult);
            Assert.Equal(1, blogResult.Id);
            Assert.Equal("/a/1", blogResult.Url);
        }

        [Fact]
        public void GetBlogByIdProjection()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var query = repository.SingleResultQuery()
                                  .AndFilter(blog => blog.Id == 1)
                                  .Select(selector => new
                                  {
                                      selector.Id,
                                      Name = selector.Title,
                                      Link = selector.Url,
                                      Type = selector.Type.Description
                                  });

            var blogResult = repository.SingleOrDefault(query);

            Assert.NotNull(blogResult);
            Assert.Equal(1, blogResult.Id);
            Assert.Equal("a1", blogResult.Name);
            Assert.Equal("/a/1", blogResult.Link);
            Assert.Equal("z1", blogResult.Type);
        }

        [Fact]
        public void ExistsBlog()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var exists = repository.Any(blog => blog.Url.StartsWith("/a/"));

            Assert.True(exists);
        }

        [Fact]
        public void GetBlogCount()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var count = repository.Count();

            Assert.Equal(50, count);

            var longCount = repository.LongCount();

            Assert.Equal(50, longCount);
        }

        [Fact]
        public void MaxBlogId()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var id = repository.Max(blog => blog.Id);

            Assert.Equal(50, id);
        }

        [Fact]
        public void MinBlogId()
        {
            var repository = _unitOfWorkOfT.Repository<Blog>();

            var id = repository.Min(blog => blog.Id);

            Assert.Equal(1, id);
        }

        [Fact]
        public void UpdateManyBlogs()
        {
            // Arrange
            var blogIds = new List<int> { 10, 11 };
            var newBlogTitle = "a-updated-title";

            // Act
            var repository = _unitOfWorkOfT.Repository<Blog>();

            repository.UpdateMany(
                blog => blogIds.Contains(blog.Id),
                new Dictionary<Expression<Func<Blog, object>>, object>
                {
                    { blog => blog.Title, newBlogTitle }
                });

            _unitOfWorkOfT.SaveChanges();

            // Assert
            var query = repository.MultipleResultQuery()
                .AndFilter(blog => blogIds.Contains(blog.Id));

            var blogResults = repository.Search(query);

            Assert.Equal(blogResults.SingleOrDefault(blog => blog.Id == blogIds.First())?.Title, newBlogTitle);
            Assert.Equal(blogResults.SingleOrDefault(blog => blog.Id == blogIds.Last())?.Title, newBlogTitle);
        }

        [Fact]
        public void BulkWriteBlogs()
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

            repository.BulkWrite(requests);

            _unitOfWorkOfT.SaveChanges();

            // Assert
            var query = repository.MultipleResultQuery()
                .AndFilter(blog => blogIds.Contains(blog.Id));

            var blogResults = repository.Search(query);

            Assert.Equal(blogResults.SingleOrDefault(blog => blog.Id == blogIds.First())?.Title, $"a{blogIds.First()}-{newBlogTitle}");
            Assert.Equal(blogResults.SingleOrDefault(blog => blog.Id == blogIds.Last())?.Title, $"a{blogIds.Last()}-{newBlogTitle}");
        }

        [Fact]
        public void AddUnsuccessfulBlogWithinTransaction()
        {
            const int Id = 51;

            var repository = _unitOfWorkOfT.Repository<Blog>();

            var blog = Seeder.SeedBlog(Id);

            _unitOfWorkOfT.StartTransaction();

            var insertOneResult = repository.InsertOne(blog);

            blog.Title += " - Updated";

            var UpdateOneResult = repository.UpdateOne(
                x => x.Id == Id,
                blog,
                new Expression<Func<Blog, object>>[] { x => x.Title });

            var saveChangesResult = _unitOfWorkOfT.SaveChanges();

            _unitOfWorkOfT.AbortTransaction();

            var id = repository.Max(x => x.Id);

            Assert.Equal(50, id);
        }

        [Fact]
        public void CustomRepository()
        {
            var customRepository = _unitOfWorkOfT.CustomRepository<ICustomBlogRepository>();

            Assert.NotNull(customRepository);
        }
    }
}
