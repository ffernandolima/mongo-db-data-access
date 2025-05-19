using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Data;
using MongoDB.Data.Repositories;
using MongoDB.Data.Repositories.Interfaces;
using MongoDB.Infrastructure;
using MongoDB.Infrastructure.Extensions;
using MongoDB.Models;
using MongoDB.Repository.Extensions;
using MongoDB.Tests.Dummies;
using MongoDB.UnitOfWork;
using MongoDB.UnitOfWork.Abstractions.Extensions;
using System;

namespace MongoDB.Tests.Fixtures
{
    public sealed class InfrastructureFixture
    {
        public IConfiguration Configuration { get; }
        public IServiceProvider ServiceProvider { get; }

        public InfrastructureFixture()
        {
            var services = new ServiceCollection();

            Configuration = BuildConfiguration();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            Seed();
        }

        private static IConfiguration BuildConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            return configuration;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddMongoDbContext<IMongoDbContext, BloggingContext>(
                connectionString: Configuration.GetValue<string>("MongoSettings:Blogging:ConnectionString"),
                databaseName: Configuration.GetValue<string>("MongoSettings:Blogging:DatabaseName"),
                setupFluentConfigurationOptions: options => options.ScanningAssemblies = new[] { typeof(BloggingContext).Assembly });

            for (int idx = 1; idx <= 2; idx++)
            {
                services.AddMongoDbContext<IMongoDbContext, TestingContext>(
                    connectionString: Configuration.GetValue<string>("MongoSettings:Testing:ConnectionString"),
                    databaseName: Configuration.GetValue<string>("MongoSettings:Testing:DatabaseName"),
                    setupDbContextOptions: options => options.DbContextId = $"{nameof(TestingContext)} - {idx}",
                    setupFluentConfigurationOptions: options => options.ScanningAssemblies = new[] { typeof(TestingContext).Assembly });
            }

            services.AddMongoDbUnitOfWork();
            services.AddMongoDbUnitOfWork<BloggingContext>();
            services.AddMongoDbUnitOfWork<TestingContext>();

            services.AddMongoDbUnitOfWork<TestingContext>($"{nameof(TestingContext)} - 1");
            services.AddMongoDbUnitOfWork<TestingContext>($"{nameof(TestingContext)} - 2");

            services.AddCustomMongoDbRepository<ICustomBlogRepository, CustomBlogRepository>();

            services.AddScoped<ITestingService, TestingService>();
        }

        private void Seed()
        {
            var unitOfWork = ServiceProvider.GetRequiredService<IMongoDbUnitOfWork<BloggingContext>>();

            var repository = unitOfWork.Repository<Blog>();

            if (!repository.Any())
            {
                var blogs = Seeder.SeedBlogs();

                repository.InsertMany(blogs);

                unitOfWork.SaveChanges();
            }
        }
    }
}
