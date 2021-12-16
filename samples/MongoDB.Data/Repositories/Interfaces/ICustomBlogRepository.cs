using MongoDB.Models;
using MongoDB.Repository;
using System.Collections.Generic;

namespace MongoDB.Data.Repositories.Interfaces
{
    public interface ICustomBlogRepository : IMongoDbRepository<Blog>
    {
        IList<string> GetAllBlogUrls();
    }
}
