using Microsoft.Extensions.DependencyInjection;
using System;

namespace MongoDB.UnitOfWork
{
    public class MongoDbServiceFactory : IMongoDbServiceFactory, IDisposable
    {
        private readonly Lazy<IServiceScope> _serviceScope;

        public MongoDbServiceFactory(IServiceScopeFactory serviceScopeFactory)
        {
            if (serviceScopeFactory == null)
            {
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            }

            _serviceScope = new Lazy<IServiceScope>(() => serviceScopeFactory.CreateScope(), isThreadSafe: true);
        }

        public T GetService<T>() where T : class
        {
            var service = _serviceScope.Value.ServiceProvider.GetService<T>();

            return service;
        }

        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_serviceScope.IsValueCreated)
                    {
                        _serviceScope.Value.Dispose();
                    }
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}
