using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using MongoDB.Infrastructure.Internal;
using System;

namespace MongoDB.Infrastructure.Extensions
{
    public static class MongoDbInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            MongoClientSettings clientSettings,
            string databaseName,
            MongoDatabaseSettings databaseSettings = null,
            MongoDbContextOptions dbContextOptions = null,
            MongoDbKeepAliveSettings keepAliveSettings = null,
            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            Func<IServiceProvider, IMongoClient, IMongoDatabase, IMongoDbContextOptions, object> factory = null)
                where TService : IMongoDbContext
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (clientSettings is null)
            {
                throw new ArgumentNullException(nameof(clientSettings), $"{nameof(clientSettings)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException($"{nameof(databaseName)} cannot be null or whitespace.", nameof(databaseName));
            }

            clientSettings.ConfigureCluster(keepAliveSettings);

            if (factory != null)
            {
                var client = new MongoClient(clientSettings);
                var database = client.GetDatabase(databaseName, databaseSettings);
                var options = new MongoDbContextOptions(clientSettings);
                 
                services.TryAdd(new ServiceDescriptor(typeof(TService), sp => factory(sp, client, database, options), serviceLifetime));
            }
            else
            {
                services.TryAddSingleton<IMongoClient>(new MongoClient(clientSettings));
                services.TryAddSingleton(provider =>
                {
                    var client = provider.GetRequiredService<IMongoClient>();
                    var database = client.GetDatabase(databaseName, databaseSettings);
                    return database;
                });
                services.TryAddSingleton<IMongoDbContextOptions>(dbContextOptions ?? new MongoDbContextOptions(clientSettings));
                services.TryAdd(new ServiceDescriptor(typeof(TService), typeof(TImplementation), serviceLifetime));                
            }

            if (typeof(TService) != typeof(TImplementation))
            {
                services.TryAdd(
                    new ServiceDescriptor(
                        typeof(TImplementation),
                        provider => (TImplementation)provider.GetService<TService>(),
                        serviceLifetime));
            }

            if (fluentConfigurationOptions?.EnableAssemblyScanning ?? false)
            {
                MongoDbFluentConfigurator.ApplyConfigurationsFromAssemblies(
                    fluentConfigurationOptions.ScanningAssemblies,
                    fluentConfigurationOptions.ScanningFilter);
            }

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            Action<MongoClientSettings> setupClientSettings,
            string databaseName,
            Action<MongoDatabaseSettings> setupDatabaseSettings = null,
            Action<MongoDbContextOptions> setupDbContextOptions = null,
            Action<MongoDbKeepAliveSettings> setupKeepAliveSettings = null,
            Action<MongoDbFluentConfigurationOptions> setupFluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            Func<IServiceProvider, IMongoClient, IMongoDatabase, IMongoDbContextOptions, object> factory = null)
                where TService : IMongoDbContext
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (setupClientSettings is null)
            {
                throw new ArgumentNullException(nameof(setupClientSettings), $"{nameof(setupClientSettings)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException($"{nameof(databaseName)} cannot be null or whitespace.", nameof(databaseName));
            }

            var clientSettings = new MongoClientSettings();
            setupClientSettings.Invoke(clientSettings);

            MongoDatabaseSettings databaseSettings = null;
            if (setupDatabaseSettings is not null)
            {
                databaseSettings = new MongoDatabaseSettings();
                setupDatabaseSettings.Invoke(databaseSettings);
            }

            MongoDbContextOptions dbContextOptions = null;
            if (setupDbContextOptions is not null)
            {
                dbContextOptions = new MongoDbContextOptions(clientSettings);
                setupDbContextOptions.Invoke(dbContextOptions);
            }

            MongoDbKeepAliveSettings keepAliveSettings = null;
            if (setupKeepAliveSettings is not null)
            {
                keepAliveSettings = new MongoDbKeepAliveSettings();
                setupKeepAliveSettings.Invoke(keepAliveSettings);
            }

            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null;
            if (setupFluentConfigurationOptions is not null)
            {
                fluentConfigurationOptions = new MongoDbFluentConfigurationOptions();
                setupFluentConfigurationOptions.Invoke(fluentConfigurationOptions);
            }

            services.AddMongoDbContext<TService, TImplementation>(
                clientSettings,
                databaseName,
                databaseSettings,
                dbContextOptions,
                keepAliveSettings,
                fluentConfigurationOptions,
                serviceLifetime,
                factory);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            MongoUrl url,
            string databaseName,
            MongoDatabaseSettings databaseSettings = null,
            MongoDbContextOptions dbContextOptions = null,
            MongoDbKeepAliveSettings keepAliveSettings = null,
            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            Func<IServiceProvider, IMongoClient, IMongoDatabase, IMongoDbContextOptions, object> factory = null)
                where TService : IMongoDbContext
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (url is null)
            {
                throw new ArgumentNullException(nameof(url), $"{nameof(url)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException($"{nameof(databaseName)} cannot be null or whitespace.", nameof(databaseName));
            }

            services.AddMongoDbContext<TService, TImplementation>(
                MongoClientSettings.FromUrl(url),
                databaseName,
                databaseSettings,
                dbContextOptions,
                keepAliveSettings,
                fluentConfigurationOptions,
                serviceLifetime,
                factory);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            MongoUrl url,
            string databaseName,
            Action<MongoDatabaseSettings> setupDatabaseSettings = null,
            Action<MongoDbContextOptions> setupDbContextOptions = null,
            Action<MongoDbKeepAliveSettings> setupKeepAliveSettings = null,
            Action<MongoDbFluentConfigurationOptions> setupFluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            Func<IServiceProvider, IMongoClient, IMongoDatabase, IMongoDbContextOptions, object> factory = null)
                where TService : IMongoDbContext
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (url is null)
            {
                throw new ArgumentNullException(nameof(url), $"{nameof(url)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException($"{nameof(databaseName)} cannot be null or whitespace.", nameof(databaseName));
            }

            var clientSettings = MongoClientSettings.FromUrl(url);

            MongoDatabaseSettings databaseSettings = null;
            if (setupDatabaseSettings is not null)
            {
                databaseSettings = new MongoDatabaseSettings();
                setupDatabaseSettings.Invoke(databaseSettings);
            }

            MongoDbContextOptions dbContextOptions = null;
            if (setupDbContextOptions is not null)
            {
                dbContextOptions = new MongoDbContextOptions(clientSettings);
                setupDbContextOptions.Invoke(dbContextOptions);
            }

            MongoDbKeepAliveSettings keepAliveSettings = null;
            if (setupKeepAliveSettings is not null)
            {
                keepAliveSettings = new MongoDbKeepAliveSettings();
                setupKeepAliveSettings.Invoke(keepAliveSettings);
            }

            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null;
            if (setupFluentConfigurationOptions is not null)
            {
                fluentConfigurationOptions = new MongoDbFluentConfigurationOptions();
                setupFluentConfigurationOptions.Invoke(fluentConfigurationOptions);
            }

            services.AddMongoDbContext<TService, TImplementation>(
                clientSettings,
                databaseName,
                databaseSettings,
                dbContextOptions,
                keepAliveSettings,
                fluentConfigurationOptions,
                serviceLifetime, 
                factory);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            MongoUrlBuilder urlBuilder,
            string databaseName,
            MongoDatabaseSettings databaseSettings = null,
            MongoDbContextOptions dbContextOptions = null,
            MongoDbKeepAliveSettings keepAliveSettings = null,
            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            Func<IServiceProvider, IMongoClient, IMongoDatabase, IMongoDbContextOptions, object> factory = null)
                where TService : IMongoDbContext
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (urlBuilder is null)
            {
                throw new ArgumentNullException(nameof(urlBuilder), $"{nameof(urlBuilder)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException($"{nameof(databaseName)} cannot be null or whitespace.", nameof(databaseName));
            }

            services.AddMongoDbContext<TService, TImplementation>(
                urlBuilder.ToMongoUrl(),
                databaseName,
                databaseSettings,
                dbContextOptions,
                keepAliveSettings,
                fluentConfigurationOptions,
                serviceLifetime,
                factory);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            Action<MongoUrlBuilder> setupUrlBuilder,
            string databaseName,
            Action<MongoDatabaseSettings> setupDatabaseSettings = null,
            Action<MongoDbContextOptions> setupDbContextOptions = null,
            Action<MongoDbKeepAliveSettings> setupKeepAliveSettings = null,
            Action<MongoDbFluentConfigurationOptions> setupFluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            Func<IServiceProvider, IMongoClient, IMongoDatabase, IMongoDbContextOptions, object> factory = null)
                where TService : IMongoDbContext
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (setupUrlBuilder is null)
            {
                throw new ArgumentNullException(nameof(setupUrlBuilder), $"{nameof(setupUrlBuilder)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException($"{nameof(databaseName)} cannot be null or whitespace.", nameof(databaseName));
            }

            var urlBuilder = new MongoUrlBuilder();
            setupUrlBuilder.Invoke(urlBuilder);

            services.AddMongoDbContext<TService, TImplementation>(
                urlBuilder.ToMongoUrl(),
                databaseName,
                setupDatabaseSettings,
                setupDbContextOptions,
                setupKeepAliveSettings,
                setupFluentConfigurationOptions,
                serviceLifetime);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            string connectionString,
            string databaseName,
            MongoDatabaseSettings databaseSettings = null,
            MongoDbContextOptions dbContextOptions = null,
            MongoDbKeepAliveSettings keepAliveSettings = null,
            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            Func<IServiceProvider, IMongoClient, IMongoDatabase, IMongoDbContextOptions, object> factory = null)
                where TService : IMongoDbContext
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"{nameof(connectionString)} cannot be null or whitespace.", nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException($"{nameof(databaseName)} cannot be null or whitespace.", nameof(databaseName));
            }

            services.AddMongoDbContext<TService, TImplementation>(
                MongoClientSettings.FromConnectionString(connectionString),
                databaseName,
                databaseSettings,
                dbContextOptions,
                keepAliveSettings,
                fluentConfigurationOptions,
                serviceLifetime,
                factory);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            string connectionString,
            string databaseName,
            Action<MongoDatabaseSettings> setupDatabaseSettings = null,
            Action<MongoDbContextOptions> setupDbContextOptions = null,
            Action<MongoDbKeepAliveSettings> setupKeepAliveSettings = null,
            Action<MongoDbFluentConfigurationOptions> setupFluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            Func<IServiceProvider, IMongoClient, IMongoDatabase, IMongoDbContextOptions, object> factory = null)
                where TService : IMongoDbContext
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"{nameof(connectionString)} cannot be null or whitespace.", nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException($"{nameof(databaseName)} cannot be null or whitespace.", nameof(databaseName));
            }

            var clientSettings = MongoClientSettings.FromConnectionString(connectionString);

            MongoDatabaseSettings databaseSettings = null;
            if (setupDatabaseSettings is not null)
            {
                databaseSettings = new MongoDatabaseSettings();
                setupDatabaseSettings.Invoke(databaseSettings);
            }

            MongoDbContextOptions dbContextOptions = null;
            if (setupDbContextOptions is not null)
            {
                dbContextOptions = new MongoDbContextOptions(clientSettings);
                setupDbContextOptions.Invoke(dbContextOptions);
            }

            MongoDbKeepAliveSettings keepAliveSettings = null;
            if (setupKeepAliveSettings is not null)
            {
                keepAliveSettings = new MongoDbKeepAliveSettings();
                setupKeepAliveSettings.Invoke(keepAliveSettings);
            }

            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null;
            if (setupFluentConfigurationOptions is not null)
            {
                fluentConfigurationOptions = new MongoDbFluentConfigurationOptions();
                setupFluentConfigurationOptions.Invoke(fluentConfigurationOptions);
            }

            services.AddMongoDbContext<TService, TImplementation>(
                clientSettings,
                databaseName,
                databaseSettings,
                dbContextOptions,
                keepAliveSettings,
                fluentConfigurationOptions,
                serviceLifetime,
                factory);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            IConfiguration configuration,
            MongoDbContextOptions dbContextOptions = null,
            MongoDbKeepAliveSettings keepAliveSettings = null,
            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            Func<IServiceProvider, IMongoClient, IMongoDatabase, IMongoDbContextOptions, object> factory = null)
                where TService : IMongoDbContext
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration), $"{nameof(configuration)} cannot be null.");
            }

            var clientSettings = configuration.GetMongoClientSettings();
            var connectionString = configuration.GetConnectionString();

            switch (clientSettings, connectionString)
            {
                case (clientSettings: null, connectionString: null):
                    throw new ArgumentException($"No {nameof(clientSettings)} or {nameof(connectionString)} have been set up.");

                case (clientSettings: not null, connectionString: not null):
                    throw new ArgumentException($"Both {nameof(clientSettings)} and {nameof(connectionString)} have been set up.");
            }

            if (clientSettings is not null)
            {
                services.AddMongoDbContext<TService, TImplementation>(
                    clientSettings,
                    configuration.GetDatabaseName(),
                    configuration.GetMongoDatabaseSettings(),
                    dbContextOptions,
                    keepAliveSettings,
                    fluentConfigurationOptions,
                    serviceLifetime,
                    factory);
            }

            if (connectionString is not null)
            {
                services.AddMongoDbContext<TService, TImplementation>(
                    connectionString,
                    configuration.GetDatabaseName(),
                    configuration.GetMongoDatabaseSettings(),
                    dbContextOptions,
                    keepAliveSettings,
                    fluentConfigurationOptions,
                    serviceLifetime, 
                    factory);
            }

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<MongoDbContextOptions> setupDbContextOptions = null,
            Action<MongoDbKeepAliveSettings> setupKeepAliveSettings = null,
            Action<MongoDbFluentConfigurationOptions> setupFluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            Func<IServiceProvider, IMongoClient, IMongoDatabase, IMongoDbContextOptions, object> factory = null)
                where TService : IMongoDbContext
                where TImplementation : class, TService
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services), $"{nameof(services)} cannot be null.");
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration), $"{nameof(configuration)} cannot be null.");
            }

            var clientSettings = configuration.GetMongoClientSettings();
            var connectionString = configuration.GetConnectionString();

            switch (clientSettings, connectionString)
            {
                case (clientSettings: null, connectionString: null):
                    throw new ArgumentException($"No {nameof(clientSettings)} or {nameof(connectionString)} have been set up.");

                case (clientSettings: not null, connectionString: not null):
                    throw new ArgumentException($"Both {nameof(clientSettings)} and {nameof(connectionString)} have been set up.");
            }

            MongoDbContextOptions dbContextOptions = null;
            if (setupDbContextOptions is not null)
            {
                dbContextOptions = new MongoDbContextOptions(clientSettings);
                setupDbContextOptions.Invoke(dbContextOptions);
            }

            MongoDbKeepAliveSettings keepAliveSettings = null;
            if (setupKeepAliveSettings is not null)
            {
                keepAliveSettings = new MongoDbKeepAliveSettings();
                setupKeepAliveSettings.Invoke(keepAliveSettings);
            }

            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null;
            if (setupFluentConfigurationOptions is not null)
            {
                fluentConfigurationOptions = new MongoDbFluentConfigurationOptions();
                setupFluentConfigurationOptions.Invoke(fluentConfigurationOptions);
            }

            if (clientSettings is not null)
            {
                services.AddMongoDbContext<TService, TImplementation>(
                    clientSettings,
                    configuration.GetDatabaseName(),
                    configuration.GetMongoDatabaseSettings(),
                    dbContextOptions,
                    keepAliveSettings,
                    fluentConfigurationOptions,
                    serviceLifetime,
                    factory);
            }

            if (connectionString is not null)
            {
                services.AddMongoDbContext<TService, TImplementation>(
                    connectionString,
                    configuration.GetDatabaseName(),
                    configuration.GetMongoDatabaseSettings(),
                    dbContextOptions,
                    keepAliveSettings,
                    fluentConfigurationOptions,
                    serviceLifetime,
                    factory);
            }

            return services;
        }
    }
}
