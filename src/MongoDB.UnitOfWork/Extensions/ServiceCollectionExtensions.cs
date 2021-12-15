using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Infrastructure.Abstractions;
using System;

namespace MongoDB.UnitOfWork.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbUnitOfWork(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        services.TryAddSingleton<IMongoDbServiceFactory, MongoDbServiceFactory>();
                        services.TryAddSingleton<IMongoDbRepositoryFactory, MongoDbUnitOfWork>();
                        services.TryAddSingleton<IMongoDbUnitOfWork, MongoDbUnitOfWork>();
                    }
                    break;
                case ServiceLifetime.Scoped:
                    {
                        services.TryAddScoped<IMongoDbServiceFactory, MongoDbServiceFactory>();
                        services.TryAddScoped<IMongoDbRepositoryFactory, MongoDbUnitOfWork>();
                        services.TryAddScoped<IMongoDbUnitOfWork, MongoDbUnitOfWork>();
                    }
                    break;
                case ServiceLifetime.Transient:
                    {
                        services.TryAddTransient<IMongoDbServiceFactory, MongoDbServiceFactory>();
                        services.TryAddTransient<IMongoDbRepositoryFactory, MongoDbUnitOfWork>();
                        services.TryAddTransient<IMongoDbUnitOfWork, MongoDbUnitOfWork>();
                    }
                    break;
                default:
                    break;
            }

            return services;
        }

        public static IServiceCollection AddMongoDbUnitOfWork<T>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where T : class, IMongoDbContext
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        services.TryAddSingleton<IMongoDbServiceFactory, MongoDbServiceFactory>();
                        services.TryAddSingleton<IMongoDbRepositoryFactory<T>, MongoDbUnitOfWork<T>>();
                        services.TryAddSingleton<IMongoDbUnitOfWork<T>, MongoDbUnitOfWork<T>>();
                    }
                    break;
                case ServiceLifetime.Scoped:
                    {
                        services.TryAddScoped<IMongoDbServiceFactory, MongoDbServiceFactory>();
                        services.TryAddScoped<IMongoDbRepositoryFactory<T>, MongoDbUnitOfWork<T>>();
                        services.TryAddScoped<IMongoDbUnitOfWork<T>, MongoDbUnitOfWork<T>>();
                    }
                    break;
                case ServiceLifetime.Transient:
                    {
                        services.TryAddTransient<IMongoDbServiceFactory, MongoDbServiceFactory>();
                        services.TryAddTransient<IMongoDbRepositoryFactory<T>, MongoDbUnitOfWork<T>>();
                        services.TryAddTransient<IMongoDbUnitOfWork<T>, MongoDbUnitOfWork<T>>();
                    }
                    break;
                default:
                    break;
            }

            return services;
        }
    }
}
