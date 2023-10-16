using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Infrastructure;
using System;

namespace MongoDB.UnitOfWork.Abstractions.Extensions
{
    public static class MongoDbUnitOfWorkServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbUnitOfWork(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            services.TryAdd<IMongoDbServiceFactory, MongoDbServiceFactory>(serviceLifetime);

            services.TryAdd<IMongoDbUnitOfWork, MongoDbUnitOfWork>(serviceLifetime);

            services.TryAdd(
                ServiceDescriptor.Describe(
                    typeof(IMongoDbRepositoryFactory),
                    provider => provider.GetRequiredService<IMongoDbUnitOfWork>(),
                    serviceLifetime));

            return services;
        }

        public static IServiceCollection AddMongoDbUnitOfWork<T>(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
                where T : class, IMongoDbContext
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            services.TryAdd<IMongoDbServiceFactory, MongoDbServiceFactory>(serviceLifetime);

            services.TryAdd<IMongoDbUnitOfWorkFactory<T>, MongoDbUnitOfWorkFactory<T>>(serviceLifetime);

            services.TryAdd<IMongoDbUnitOfWork<T>, MongoDbUnitOfWork<T>>(serviceLifetime);

            services.TryAdd(
                ServiceDescriptor.Describe(
                    typeof(IMongoDbRepositoryFactory<T>),
                    provider => provider.GetRequiredService<IMongoDbUnitOfWork<T>>(),
                    serviceLifetime));

            return services;
        }

        private static void TryAdd<TService, TImplementation>(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime)
        {
            services.TryAdd(
                ServiceDescriptor.Describe(
                    typeof(TService), typeof(TImplementation), serviceLifetime));
        }
    }
}
