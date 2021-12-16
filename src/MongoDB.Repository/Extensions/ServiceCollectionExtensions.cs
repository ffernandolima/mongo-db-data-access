using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Repository.Internal;
using System;

namespace MongoDB.Repository.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomMongoDbRepository<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TService : class, IMongoDbRepository
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (!(typeof(TImplementation).BaseType?.IsGenericType(typeof(MongoDbRepository<>)) ?? false))
            {
                throw new ArgumentException("Implementation constraint has not been satisfied.");
            }

            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        services.TryAddSingleton<TService, TImplementation>();
                    }
                    break;
                case ServiceLifetime.Scoped:
                    {
                        services.TryAddScoped<TService, TImplementation>();
                    }
                    break;
                case ServiceLifetime.Transient:
                    {
                        services.TryAddTransient<TService, TImplementation>();
                    }
                    break;
                default:
                    break;
            }

            return services;
        }

        public static IServiceCollection AddMongoDbRepository<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TService : class, IMongoDbRepository
            where TImplementation : class, TService
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (!typeof(TImplementation).IsGenericType(typeof(MongoDbRepository<>)))
            {
                throw new ArgumentException("Implementation constraint has not been satisfied.");
            }

            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        services.TryAddSingleton<TService, TImplementation>();
                    }
                    break;
                case ServiceLifetime.Scoped:
                    {
                        services.TryAddScoped<TService, TImplementation>();
                    }
                    break;
                case ServiceLifetime.Transient:
                    {
                        services.TryAddTransient<TService, TImplementation>();
                    }
                    break;
                default:
                    break;
            }

            return services;
        }
    }
}
