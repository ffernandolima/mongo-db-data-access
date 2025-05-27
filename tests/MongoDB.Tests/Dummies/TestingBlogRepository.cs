using MongoDB.Driver;
using MongoDB.Models;
using MongoDB.Repository;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Tests.Dummies
{
    public interface ITestingBlogRepository : IMongoDbRepository<Blog>
    {
        IList<string> GetAllBlogUrls();
    }

    internal class TestingBlogRepository : MongoDbRepository<Blog>, ITestingBlogRepository
    {
        public TestingBlogRepository(TestingContext context)
            : base(context)
        { }

        public TestingBlogRepository(TestingContext context, MongoDbRepositoryOptions<Blog> options)
            : base(context, options)
        { }

        public IList<string> GetAllBlogUrls()
            => Context.GetCollection<Blog>()
                      .AsQueryable()
                      .Select(blog => blog.Url)
                      .ToList();
    }
}
