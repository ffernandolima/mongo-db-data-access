using MongoDB.Data.Repositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Models;
using MongoDB.Repository;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Data.Repositories
{
    public class CustomBlogRepository : MongoDbRepository<Blog>, ICustomBlogRepository
    {
        public CustomBlogRepository(BloggingContext context)
            : base(context)
        { }

        public CustomBlogRepository(BloggingContext context, MongoDbRepositoryOptions<Blog> options)
            : base(context, options)
        { }

        public IList<string> GetAllBlogUrls()
            => Context.GetCollection<Blog>()
                      .AsQueryable()
                      .Select(blog => blog.Url)
                      .ToList();
    }
}
