using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using System;

namespace MongoDB.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            return AddMongoDbContextInternal<TService, TImplementation>(services, implementationFactory, serviceLifetime);
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, MongoClientSettings clientSettings, string databaseName, MongoDatabaseSettings databaseSettings = null, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { clientSettings, databaseName, databaseSettings }, serviceLifetime);
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, Action<MongoClientSettings> configureClientSettings, string databaseName, Action<MongoDatabaseSettings> configureDatabaseSettings = null, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { clientSettings, databaseName, databaseSettings }, serviceLifetime);
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, MongoUrl url, string databaseName, MongoDatabaseSettings databaseSettings = null, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { url, databaseName, databaseSettings }, serviceLifetime);
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, string connectionString, string databaseName, MongoDatabaseSettings databaseSettings = null, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { connectionString, databaseName, databaseSettings }, serviceLifetime);
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, string connectionString, string databaseName, Action<MongoDatabaseSettings> configureDatabaseSettings = null, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { connectionString, databaseName, databaseSettings }, serviceLifetime);
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(this IServiceCollection services, IConfiguration configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            return AddMongoDbContextInternal<TService, TImplementation>(services, new object[] { configuration }, serviceLifetime);
        }

        private static IServiceCollection AddMongoDbContextInternal<TService, TImplementation>(IServiceCollection services, object[] parameters, ServiceLifetime serviceLifetime)
            where TService : class
            where TImplementation : class, TService
        {
            TImplementation ImplementationFactory(IServiceProvider provider) => ActivatorUtilities.CreateInstance<TImplementation>(provider, parameters);

            return AddMongoDbContextInternal<TService, TImplementation>(services, ImplementationFactory, serviceLifetime);
        }

        private static IServiceCollection AddMongoDbContextInternal<TService, TImplementation>(IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory, ServiceLifetime serviceLifetime)
            where TService : class
            where TImplementation : class, TService
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        services.TryAddSingleton<TService>(provider => implementationFactory(provider));
                        services.TryAddSingleton(provider => implementationFactory(provider));
                    }
                    break;
                case ServiceLifetime.Scoped:
                    {
                        services.TryAddScoped<TService>(provider => implementationFactory(provider));
                        services.TryAddScoped(provider => implementationFactory(provider));
                    }
                    break;
                case ServiceLifetime.Transient:
                    {
                        services.TryAddTransient<TService>(provider => implementationFactory(provider));
                        services.TryAddTransient(provider => implementationFactory(provider));
                    }
                    break;
                default:
                    break;
            }

            return services;
        }
    }
}
