using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

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

        /// <remarks>
        /// Control sending TCP keep-alive packets and the interval at which they are sent.
        /// This control code is supported on Windows 2000 and later operating systems.
        /// </remarks>        
        public static ClusterBuilder ConfigureTcp(this ClusterBuilder clusterBuilder, MongoDbKeepAliveSettings keepAliveSettings)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return clusterBuilder;
            }

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
