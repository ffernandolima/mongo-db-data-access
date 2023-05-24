using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MongoDB.Infrastructure
{
    public class MongoDbServiceFactory : IMongoDbServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MongoDbServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider), $"{nameof(serviceProvider)} cannot be null.");
        }

        public T GetService<T>() where T : class
        {
            var service = _serviceProvider.GetService<T>();

            return service;
        }

        public IEnumerable<T> GetServices<T>() where T : class
        {
            var services = _serviceProvider.GetServices<T>();

            return services;
        }
    }
}
