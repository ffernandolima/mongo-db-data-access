using Microsoft.Extensions.DependencyInjection;
using MongoDB.Data;
using MongoDB.Data.Repositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Models;
using MongoDB.QueryBuilder;
using MongoDB.Repository.Extensions;
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

        private readonly IMongoDbRepositoryFactory _repositoryFactory;
        private readonly IMongoDbRepositoryFactory<BloggingContext> _repositoryFactoryOfT;

        public SyncDataAccessTests(InfrastructureFixture infrastructureFixture)
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
        public void UpdateManyBlogs()
        {
            // Arrange
            var idBlog1 = 14;
            var idBlog2 = 15;
            var blogsId = new List<int> { idBlog1, idBlog2 };
            var updatedTitle = "a-updated-title";

            var filter = Builders<Blog>.Filter.In(b => b.Id, blogsId);
            var update = Builders<Blog>.Update.Set(b => b.Title, updatedTitle);

            // Act
            var repository = _unitOfWork.Repository<Blog>();

            repository.UpdateMany(filter, update);
            _unitOfWork.SaveChanges();

            // Assert
            var query = repository
                .MultipleResultQuery()
                .AndFilter(r => blogsId.Contains(r.Id));
            var result = repository.Search(query);

            Assert.Equal(result.First(r => r.Id == idBlog1)?.Title, updatedTitle);
            Assert.Equal(result.First(r => r.Id == idBlog2)?.Title, updatedTitle);
        }

        [Fact]
        public void BulkWriteBlogsAsync()
        {
            // Arrange
            var idBlog1 = 20;
            var idBlog2 = 21;
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

            repository.BulkWrite(requests);
            _unitOfWork.SaveChanges();

            // Assert
            var query = repository
                .MultipleResultQuery()
                .AndFilter(r => blogsId.Contains(r.Id));
            var result = repository.Search(query);

            Assert.Equal(result.First(r => r.Id == idBlog1)?.Title, $"{updatedTitle}-{idBlog1}");
            Assert.Equal(result.First(r => r.Id == idBlog2)?.Title, $"{updatedTitle}-{idBlog2}");
        }

        [Fact]
        public void GetAllBlogs()
        {
            var repository = _unitOfWork.Repository<Blog>();

            var query = repository.MultipleResultQuery();

            var blogs = repository.Search(query);

            Assert.NotNull(blogs);
            Assert.Equal(50, blogs.Count);
        }

        [Fact]
        public void GetAllBlogsProjection()
        {
            var repository = _unitOfWork.Repository<Blog>();

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
            var repository = _unitOfWork.Repository<Blog>();

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
            var repository = _unitOfWork.Repository<Blog>();

            var query = repository.MultipleResultQuery()
                                  .Top(10);

            var blogs = repository.Search(query);

            Assert.NotNull(blogs);
            Assert.Equal(10, blogs.Count);
        }

        [Fact]
        public void GetPagedBlogs()
        {
            var repository = _unitOfWork.Repository<Blog>();

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
            var repository = _unitOfWork.Repository<Blog>();

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
            var repository = _unitOfWork.Repository<Blog>();

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
            var repository = _unitOfWork.Repository<Blog>();

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
            var repository = _unitOfWork.Repository<Blog>();

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
            var repository = _unitOfWork.Repository<Blog>();

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
            var repository = _unitOfWork.Repository<Blog>();

            var exists = repository.Any(blog => blog.Url.StartsWith("/a/"));

            Assert.True(exists);
        }

        [Fact]
        public void GetBlogCount()
        {
            var repository = _unitOfWork.Repository<Blog>();

            var count = repository.Count();

            Assert.Equal(50, count);

            var longCount = repository.LongCount();

            Assert.Equal(50, longCount);
        }

        [Fact]
        public void MaxBlogId()
        {
            var repository = _unitOfWork.Repository<Blog>();

            var id = repository.Max(blog => blog.Id);

            Assert.Equal(50, id);
        }

        [Fact]
        public void MinBlogId()
        {
            var repository = _unitOfWork.Repository<Blog>();

            var id = repository.Min(blog => blog.Id);

            Assert.Equal(1, id);
        }

        [Fact]
        public void AddUnsuccessfulBlogWithinTransaction()
        {
            const int Id = 51;

            var repository = _unitOfWork.Repository<Blog>();

            var blog = Seeder.SeedBlog(Id);

            _unitOfWork.StartTransaction();

            var insertOneResult = repository.InsertOne(blog);

            blog.Title += " - Updated";

            var UpdateOneResult = repository.UpdateOne(
                x => x.Id == Id,
                blog,
                new Expression<Func<Blog, object>>[] { x => x.Title });

            var saveChangesResult = _unitOfWork.SaveChanges();

            _unitOfWork.AbortTransaction();

            var id = repository.Max(x => x.Id);

            Assert.Equal(50, id);
        }

        [Fact]
        public void CustomRepository()
        {
            var customRepository = _unitOfWork.CustomRepository<ICustomBlogRepository>();

            Assert.NotNull(customRepository);
        }
    }
}
