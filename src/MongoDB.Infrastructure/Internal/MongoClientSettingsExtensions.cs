using MongoDB.Driver;

namespace MongoDB.Infrastructure.Internal
{
    internal static class MongoClientSettingsExtensions
    {
        public static MongoClientSettings ConfigureCluster(
            this MongoClientSettings mongoClientSettings,
            MongoDbKeepAliveSettings keepAliveSettings = null)
        {
            if (mongoClientSettings is not null)
            {
                mongoClientSettings.ClusterConfigurator = clusterBuilder =>
                     clusterBuilder.ConfigureDiagnostics()
                                   .ConfigureTcp(keepAliveSettings);
            }

            return mongoClientSettings;
        }
    }
}
