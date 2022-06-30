using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace MongoDB.Infrastructure.Internal
{
    internal static class MongoClientSettingsExtensions
    {
        public static MongoClientSettings AddDiagnostics(this MongoClientSettings settings)
        {
            if (settings != null)
            {
                var options = new InstrumentationOptions { CaptureCommandText = true };

                settings.ClusterConfigurator = builder => builder.Subscribe(new DiagnosticsActivityEventSubscriber(options));
            }

            return settings;
        }
    }
}
