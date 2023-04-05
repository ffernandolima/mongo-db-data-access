using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using System;
using System.Net.Sockets;

namespace MongoDB.Infrastructure.Internal
{
    internal static class ClusterBuilderExtensions
    {
        public static ClusterBuilder ConfigureDiagnostics(this ClusterBuilder clusterBuilder)
        {
            clusterBuilder = clusterBuilder?.Subscribe(
                subscriber: new DiagnosticsActivityEventSubscriber(
                    options: new InstrumentationOptions { CaptureCommandText = true }));

            return clusterBuilder;
        }

        public static ClusterBuilder ConfigureTcp(this ClusterBuilder clusterBuilder, MongoDbKeepAliveSettings keepAliveSettings)
        {
            void SocketConfigurator(Socket socket)
            {
                socket?.IOControl(
                    ioControlCode: IOControlCode.KeepAliveValues,
                    optionInValue: keepAliveSettings.ToBytes(),
                    optionOutValue: null);
            }

            clusterBuilder = clusterBuilder?.ConfigureTcp(
                tcpStreamSettings => tcpStreamSettings?.With(
                    socketConfigurator: keepAliveSettings is null ? default : new Optional<Action<Socket>>(SocketConfigurator)));

            return clusterBuilder;
        }
    }
}
