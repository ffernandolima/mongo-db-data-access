using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Infrastructure;
using MongoDB.Infrastructure.Extensions;
using MongoDB.Tests.Infrastructure;
using MongoDB.Tests.Stubs;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MongoDB.Tests.Implementation
{
    public class Int64IdGeneratorTests : Startup
    {
        private readonly IMongoDbContext _context;

        public Int64IdGeneratorTests()
            : base()
        {
            _context = ServiceProvider.GetRequiredService<IMongoDbContext>();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddMongoDbContext<IMongoDbContext, MongoDbContext>(provider =>
            {
                var connectionString = Configuration.GetValue<string>("MongoSettings:ConnectionString");
                var databaseName = Configuration.GetValue<string>("MongoSettings:DatabaseName");

                var context = new MongoDbContext(connectionString, databaseName);

                return context;
            });
        }

        [Fact]
        public void WhenSaving1ItemIncrementItsId()
        {
            var id = GetCurrentId();

            var item = new StubInt64IdEntity { Name = "Testing 1" };

            var collection = _context.GetCollection<StubInt64IdEntity>("Int64IdEntities");

            collection.InsertOne(item);

            Assert.Equal(id + 1, item.Id);
        }

        [Fact]
        public void WhenSaving2ItemsIncrementTheirIds()
        {
            var id = GetCurrentId();

            var item1 = new StubInt64IdEntity { Name = "Testing 1" };
            var item2 = new StubInt64IdEntity { Name = "Testing 2" };

            var collection = _context.GetCollection<StubInt64IdEntity>("Int64IdEntities");

            collection.InsertOne(item1);
            collection.InsertOne(item2);

            Assert.Equal(id + 1, item1.Id);
            Assert.Equal(id + 2, item2.Id);
        }

        [Fact]
        public void WhenSavingABatchOfItemsIncrementTheirIds()
        {
            var id = GetCurrentId();

            var items = new List<StubInt64IdEntity>();

            for (int i = 0; i < 1000; i++)
            {
                items.Add(new StubInt64IdEntity { Name = $"Testing {i}" });
            }

            var collection = _context.GetCollection<StubInt64IdEntity>("Int64IdEntities");

            collection.InsertMany(items);

            var ids = items.Select(x => x.Id);

            for (var idx = 1; idx < 1001; idx++)
            {
                Assert.Contains(id + idx, ids);
            }
        }

        private long GetCurrentId()
        {
            var collection = _context.GetCollection<StubInt64IdEntity>("Int64IdEntities");

            var queryable = collection.AsQueryable();

            if (!queryable.Any())
            {
                return 0;
            }

            return queryable.Max(x => x.Id);
        }
    }
}
