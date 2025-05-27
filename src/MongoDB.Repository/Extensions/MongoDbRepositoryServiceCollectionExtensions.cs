using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Infrastructure;
using MongoDB.Repository.Internal;
using System;

namespace MongoDB.Repository.Extensions
{
    public static class MongoDbRepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomMongoDbRepository<TService, TImplementation>(
            this IServiceCollection services,
            string dbContextId = null,
            Func<IServiceProvider, TService> implementationFactory = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
                where TService : class, IMongoDbRepository
                where TImplementation : class, TService
        {
            return services.AddMongoDbRepositoryInternal<TService, TImplementation>(
                dbContextId,
                implementationFactory,
                serviceLifetime);
        }

        public static IServiceCollection AddMongoDbRepository<TService, TImplementation>(
            this IServiceCollection services,
            string dbContextId = null,
            Func<IServiceProvider, TService> implementationFactory = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
                where TService : class, IMongoDbRepository
                where TImplementation : class, TService
        {
            return services.AddMongoDbRepositoryInternal<TService, TImplementation>(
                dbContextId,
                implementationFactory,
                serviceLifetime);
        }

        private static IServiceCollection AddMongoDbRepositoryInternal<TService, TImplementation>(
            this IServiceCollection services,
            string dbContextId,
            Func<IServiceProvider, TService> implementationFactory,
            ServiceLifetime serviceLifetime)
                where TService : class, IMongoDbRepository
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            var serviceType = typeof(TService);
            var implementationType = typeof(TImplementation);
            var repositoryType = typeof(MongoDbRepository<>);

            if (!implementationType.InheritsFromGenericType(repositoryType))
            {
                throw new ArgumentException("The implementation type must derive from MongoDbRepository<>.");
            }

            implementationFactory ??= dbContextId is null
                ? null
                : provider => (TService)ActivatorUtilities.CreateInstance(
                    provider,
                    implementationType,
                    provider.GetKeyedService<IMongoDbContext>(dbContextId));

            var serviceDescriptor = dbContextId is null
                ? implementationFactory is null
                    ? ServiceDescriptor.Describe(
                        serviceType,
                        implementationType,
                        serviceLifetime)
                    : ServiceDescriptor.Describe(
                        serviceType,
                        provider => implementationFactory.Invoke(provider),
                        serviceLifetime)
                : ServiceDescriptor.DescribeKeyed(
                    serviceType,
                    dbContextId,
                    (provider, _) => implementationFactory.Invoke(provider),
                    serviceLifetime);

            services.TryAdd(serviceDescriptor);

            return services;
        }
    }
}
