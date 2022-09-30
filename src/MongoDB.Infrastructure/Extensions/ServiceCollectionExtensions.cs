using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using System;

namespace MongoDB.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class, IMongoDbContext
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (implementationFactory == null)
            {
                throw new ArgumentNullException(nameof(implementationFactory), $"{nameof(implementationFactory)} cannot be null.");
            }

            return AddMongoDbContextInternal<TService, TImplementation>(services, implementationFactory);
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, MongoClientSettings clientSettings, string databaseName, MongoDatabaseSettings databaseSettings = null)
            where TService : class, IMongoDbContext
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (clientSettings == null)
            {
                throw new ArgumentNullException(nameof(clientSettings));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { clientSettings, databaseName, databaseSettings });
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, Action<MongoClientSettings> configureClientSettings, string databaseName, Action<MongoDatabaseSettings> configureDatabaseSettings = null)
            where TService : class, IMongoDbContext
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (configureClientSettings == null)
            {
                throw new ArgumentNullException(nameof(configureClientSettings));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            var clientSettings = new MongoClientSettings();

            configureClientSettings.Invoke(clientSettings);

            MongoDatabaseSettings databaseSettings = null;

            if (configureDatabaseSettings != null)
            {
                databaseSettings = new MongoDatabaseSettings();

                configureDatabaseSettings.Invoke(databaseSettings);
            }

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { clientSettings, databaseName, databaseSettings });
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, MongoUrl url, string databaseName, MongoDatabaseSettings databaseSettings = null)
            where TService : class, IMongoDbContext
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { url, databaseName, databaseSettings });
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, string connectionString, string databaseName, MongoDatabaseSettings databaseSettings = null)
           where TService : class, IMongoDbContext
           where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { connectionString, databaseName, databaseSettings });
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, string connectionString, string databaseName, Action<MongoDatabaseSettings> configureDatabaseSettings = null)
            where TService : class, IMongoDbContext
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException(nameof(databaseName));
            }

            MongoDatabaseSettings databaseSettings = null;

            if (configureDatabaseSettings != null)
            {
                databaseSettings = new MongoDatabaseSettings();

                configureDatabaseSettings.Invoke(databaseSettings);
            }

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { connectionString, databaseName, databaseSettings });
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, IConfiguration configuration)
          where TService : class, IMongoDbContext
          where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { configuration });
        }

        private static IServiceCollection AddMongoDbContextInternal<TService, TImplementation>(IServiceCollection services, object[] parameters)
            where TService : class
            where TImplementation : class, TService
        {
            return AddMongoDbContextInternal<TService, TImplementation>(services, provider => ActivatorUtilities.CreateInstance<TImplementation>(provider, parameters));
        }

        private static IServiceCollection AddMongoDbContextInternal<TService, TImplementation>(IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            services.TryAddSingleton<TService>(provider => implementationFactory(provider));
            services.TryAddSingleton(provider => implementationFactory(provider));

            return services;
        }
    }
}
