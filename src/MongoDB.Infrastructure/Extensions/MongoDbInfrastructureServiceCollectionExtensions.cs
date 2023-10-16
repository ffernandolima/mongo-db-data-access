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
            MongoDbContextOptions<TImplementation> dbContextOptions = null,
            MongoDbKeepAliveSettings keepAliveSettings = null,
            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            services.TryAddSingleton<IMongoDbConnectionManager>(MongoDbConnectionManager.Instance);
            services.TryAddSingleton<IMongoDbClientManager>(MongoDbClientManager.Instance);
            services.TryAddSingleton<IMongoDbDatabaseManager>(MongoDbDatabaseManager.Instance);
            services.TryAddSingleton<IMongoDbContextOptionsManager>(MongoDbContextOptionsManager.Instance);
            services.TryAddSingleton<IMongoDbThrottlingSemaphoreManager>(MongoDbThrottlingSemaphoreManager.Instance);
            services.TryAddSingleton<IMongoDbThrottlingSemaphoreFactory>(MongoDbThrottlingSemaphoreFactory.Instance);

            services.Add(
                ServiceDescriptor.Describe(
                    typeof(TImplementation),
                    provider =>
                    {
                        var connectionManager = provider.GetRequiredService<IMongoDbConnectionManager>();

                        var connection = connectionManager.GetOrCreate(
                            clientSettings,
                            databaseName,
                            databaseSettings,
                            dbContextOptions ?? CreateDbContextOptions<TService, TImplementation>(clientSettings));

                        var context = (TImplementation)Activator.CreateInstance(
                            typeof(TImplementation),
                            connection.Client,
                            connection.Database,
                            connection.Options);

                        return context;

                    }, serviceLifetime));

            if (typeof(TService) != typeof(TImplementation))
            {
                services.Add(
                    ServiceDescriptor.Describe(
                        typeof(TService),
                        provider => provider.GetRequiredService<TImplementation>(),
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
            Action<MongoDbContextOptions<TImplementation>> setupDbContextOptions = null,
            Action<MongoDbKeepAliveSettings> setupKeepAliveSettings = null,
            Action<MongoDbFluentConfigurationOptions> setupFluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            MongoDbContextOptions<TImplementation> dbContextOptions = null;
            if (setupDbContextOptions is not null)
            {
                dbContextOptions = CreateDbContextOptions<TService, TImplementation>(clientSettings);
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
                serviceLifetime);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            MongoUrl url,
            string databaseName,
            MongoDatabaseSettings databaseSettings = null,
            MongoDbContextOptions<TImplementation> dbContextOptions = null,
            MongoDbKeepAliveSettings keepAliveSettings = null,
            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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
                serviceLifetime);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            MongoUrl url,
            string databaseName,
            Action<MongoDatabaseSettings> setupDatabaseSettings = null,
            Action<MongoDbContextOptions<TImplementation>> setupDbContextOptions = null,
            Action<MongoDbKeepAliveSettings> setupKeepAliveSettings = null,
            Action<MongoDbFluentConfigurationOptions> setupFluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            MongoDbContextOptions<TImplementation> dbContextOptions = null;
            if (setupDbContextOptions is not null)
            {
                dbContextOptions = CreateDbContextOptions<TService, TImplementation>(clientSettings);
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
                serviceLifetime);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            MongoUrlBuilder urlBuilder,
            string databaseName,
            MongoDatabaseSettings databaseSettings = null,
            MongoDbContextOptions<TImplementation> dbContextOptions = null,
            MongoDbKeepAliveSettings keepAliveSettings = null,
            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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
                serviceLifetime);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            Action<MongoUrlBuilder> setupUrlBuilder,
            string databaseName,
            Action<MongoDatabaseSettings> setupDatabaseSettings = null,
            Action<MongoDbContextOptions<TImplementation>> setupDbContextOptions = null,
            Action<MongoDbKeepAliveSettings> setupKeepAliveSettings = null,
            Action<MongoDbFluentConfigurationOptions> setupFluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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
            MongoDbContextOptions<TImplementation> dbContextOptions = null,
            MongoDbKeepAliveSettings keepAliveSettings = null,
            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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
                serviceLifetime);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            string connectionString,
            string databaseName,
            Action<MongoDatabaseSettings> setupDatabaseSettings = null,
            Action<MongoDbContextOptions<TImplementation>> setupDbContextOptions = null,
            Action<MongoDbKeepAliveSettings> setupKeepAliveSettings = null,
            Action<MongoDbFluentConfigurationOptions> setupFluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            MongoDbContextOptions<TImplementation> dbContextOptions = null;
            if (setupDbContextOptions is not null)
            {
                dbContextOptions = CreateDbContextOptions<TService, TImplementation>(clientSettings);
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
                serviceLifetime);

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            IConfiguration configuration,
            string clientSettingsSectionKey = "MongoSettings:MongoClientSettings",
            string connectionStringSectionKey = "MongoSettings:ConnectionString",
            string databaseNameSectionKey = "MongoSettings:DatabaseName",
            string databaseSettingsSectionKey = "MongoSettings:MongoDatabaseSettings",
            MongoDbContextOptions<TImplementation> dbContextOptions = null,
            MongoDbKeepAliveSettings keepAliveSettings = null,
            MongoDbFluentConfigurationOptions fluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            var clientSettings = configuration.GetMongoClientSettings(sectionKey: clientSettingsSectionKey);
            var connectionString = configuration.GetConnectionString(sectionKey: connectionStringSectionKey);

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
                    configuration.GetDatabaseName(sectionKey: databaseNameSectionKey),
                    configuration.GetMongoDatabaseSettings(sectionKey: databaseSettingsSectionKey),
                    dbContextOptions,
                    keepAliveSettings,
                    fluentConfigurationOptions,
                    serviceLifetime);
            }

            if (connectionString is not null)
            {
                services.AddMongoDbContext<TService, TImplementation>(
                    connectionString,
                    configuration.GetDatabaseName(sectionKey: databaseNameSectionKey),
                    configuration.GetMongoDatabaseSettings(sectionKey: databaseSettingsSectionKey),
                    dbContextOptions,
                    keepAliveSettings,
                    fluentConfigurationOptions,
                    serviceLifetime);
            }

            return services;
        }

        public static IServiceCollection AddMongoDbContext<TService, TImplementation>(
            this IServiceCollection services,
            IConfiguration configuration,
            string clientSettingsSectionKey = "MongoSettings:MongoClientSettings",
            string connectionStringSectionKey = "MongoSettings:ConnectionString",
            string databaseNameSectionKey = "MongoSettings:DatabaseName",
            string databaseSettingsSectionKey = "MongoSettings:MongoDatabaseSettings",
            Action<MongoDbContextOptions<TImplementation>> setupDbContextOptions = null,
            Action<MongoDbKeepAliveSettings> setupKeepAliveSettings = null,
            Action<MongoDbFluentConfigurationOptions> setupFluentConfigurationOptions = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
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

            var clientSettings = configuration.GetMongoClientSettings(sectionKey: clientSettingsSectionKey);
            var connectionString = configuration.GetConnectionString(sectionKey: connectionStringSectionKey);

            switch (clientSettings, connectionString)
            {
                case (clientSettings: null, connectionString: null):
                    throw new ArgumentException($"No {nameof(clientSettings)} or {nameof(connectionString)} have been set up.");

                case (clientSettings: not null, connectionString: not null):
                    throw new ArgumentException($"Both {nameof(clientSettings)} and {nameof(connectionString)} have been set up.");
            }

            MongoDbContextOptions<TImplementation> dbContextOptions = null;
            if (setupDbContextOptions is not null)
            {
                dbContextOptions = CreateDbContextOptions<TService, TImplementation>(clientSettings);
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
                    configuration.GetDatabaseName(sectionKey: databaseNameSectionKey),
                    configuration.GetMongoDatabaseSettings(sectionKey: databaseSettingsSectionKey),
                    dbContextOptions,
                    keepAliveSettings,
                    fluentConfigurationOptions,
                    serviceLifetime);
            }

            if (connectionString is not null)
            {
                services.AddMongoDbContext<TService, TImplementation>(
                    connectionString,
                    configuration.GetDatabaseName(sectionKey: databaseNameSectionKey),
                    configuration.GetMongoDatabaseSettings(sectionKey: databaseSettingsSectionKey),
                    dbContextOptions,
                    keepAliveSettings,
                    fluentConfigurationOptions,
                    serviceLifetime);
            }

            return services;
        }

        private static MongoDbContextOptions<TImplementation> CreateDbContextOptions<TService, TImplementation>(
            MongoClientSettings clientSettings)
                where TService : IMongoDbContext
                where TImplementation : class, TService
        {
            var contextOptionsType = typeof(MongoDbContextOptions<>).MakeGenericType(typeof(TImplementation));

            var dbContextOptions = (MongoDbContextOptions<TImplementation>)Activator.CreateInstance(
                contextOptionsType,
                clientSettings);

            return dbContextOptions;
        }
    }
}
