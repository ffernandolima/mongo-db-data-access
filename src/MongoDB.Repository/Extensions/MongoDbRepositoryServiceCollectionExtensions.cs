using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Repository.Internal;
using System;

namespace MongoDB.Repository.Extensions
{
    public static class MongoDbRepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomMongoDbRepository<TService, TImplementation>(
            this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
                where TService : class, IMongoDbRepository
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (!(typeof(TImplementation).BaseType?.IsGenericType(typeof(MongoDbRepository<>)) ?? false))
            {
                throw new ArgumentException("Implementation constraint has not been satisfied.");
            }

            services.TryAdd<TService, TImplementation>(implementationFactory, serviceLifetime);

            return services;
        }

        public static IServiceCollection AddMongoDbRepository<TService, TImplementation>(
            this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
                where TService : class, IMongoDbRepository
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (!typeof(TImplementation).IsGenericType(typeof(MongoDbRepository<>)))
            {
                throw new ArgumentException("Implementation constraint has not been satisfied.");
            }

            services.TryAdd<TService, TImplementation>(implementationFactory, serviceLifetime);

            return services;
        }

        private static void TryAdd<TService, TImplementation>(
            this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory,
            ServiceLifetime serviceLifetime)
        {
            if (implementationFactory is null)
            {
                services.TryAdd(
                    ServiceDescriptor.Describe(
                        typeof(TService), typeof(TImplementation), serviceLifetime));
            }
            else
            {
                services.TryAdd(
                    ServiceDescriptor.Describe(
                        typeof(TService),
                        provider => implementationFactory.Invoke(provider),
                        serviceLifetime));
            }
        }
    }
}
