using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace MongoDB.Infrastructure.Internal
{
    internal static class MongoDbConfigurationExtensions
    {
        public static MongoClientSettings GetMongoClientSettings(
            this IConfiguration configuration,
            string sectionKey = "MongoSettings:MongoClientSettings")
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration), $"{nameof(configuration)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(sectionKey))
            {
                throw new ArgumentException($"{nameof(sectionKey)} cannot be null or whitespace.", nameof(sectionKey));
            }

            var clientSettings = configuration.GetSection(sectionKey)?.Get<MongoClientSettings>();

            return clientSettings;
        }

        public static string GetConnectionString(
           this IConfiguration configuration,
           string sectionKey = "MongoSettings:ConnectionString")
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration), $"{nameof(configuration)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(sectionKey))
            {
                throw new ArgumentException($"{nameof(sectionKey)} cannot be null or whitespace.", nameof(sectionKey));
            }

            var databaseName = configuration.GetValue<string>(sectionKey);

            return databaseName;
        }

        public static string GetDatabaseName(
            this IConfiguration configuration,
            string sectionKey = "MongoSettings:DatabaseName")
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration), $"{nameof(configuration)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(sectionKey))
            {
                throw new ArgumentException($"{nameof(sectionKey)} cannot be null or whitespace.", nameof(sectionKey));
            }

            var databaseName = configuration.GetValue<string>(sectionKey);

            return databaseName;
        }

        public static MongoDatabaseSettings GetMongoDatabaseSettings(
            this IConfiguration configuration,
            string sectionKey = "MongoSettings:MongoDatabaseSettings")
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration), $"{nameof(configuration)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(sectionKey))
            {
                throw new ArgumentException($"{nameof(sectionKey)} cannot be null or whitespace.", nameof(sectionKey));
            }

            var databaseSettings = configuration.GetSection(sectionKey)?.Get<MongoDatabaseSettings>();

            return databaseSettings;
        }
    }
}
