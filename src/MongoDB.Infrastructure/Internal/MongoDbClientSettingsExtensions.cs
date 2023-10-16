using MongoDB.Driver;

namespace MongoDB.Infrastructure.Internal
{
    internal static class MongoDbClientSettingsExtensions
    {
        public static MongoClientSettings ConfigureCluster(
            this MongoClientSettings clientSettings,
            MongoDbKeepAliveSettings keepAliveSettings = null)
        {
            if (clientSettings is not null)
            {
                clientSettings.ClusterConfigurator = clusterBuilder =>
                     clusterBuilder.ConfigureDiagnostics()
                                   .ConfigureTcp(keepAliveSettings);
            }

            return clientSettings;
        }
    }
}
