using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MongoDB.Infrastructure.Internal
{
    internal static class MongoDbFluentConfigurator
    {
        private static readonly object _sync = new();

        public static void ApplyConfigurationsFromAssemblies(
            IEnumerable<Assembly> scanningAssemblies,
            Func<Type, bool> scanningFilter = null)
        {
            if (scanningAssemblies is null)
            {
                throw new ArgumentNullException(nameof(scanningAssemblies), $"{nameof(scanningAssemblies)} cannot be null.");
            }

            lock (_sync)
            {
                var fluentConfigurationType = typeof(IMongoDbFluentConfiguration);

                var configurationTypes = MongoDbAssemblyScanner.Scan(
                    scanningAssemblies,
                    scannedType => fluentConfigurationType.IsAssignableFrom(scannedType) &&
                        !scannedType.IsInterface &&
                        !scannedType.IsAbstract &&
                        (scanningFilter?.Invoke(scannedType) ?? true)
                );

                if (configurationTypes?.Any() ?? false)
                {
                    foreach (var configurationType in configurationTypes)
                    {
                        if (Activator.CreateInstance(configurationType) is not IMongoDbFluentConfiguration configurationInstance
                            || configurationInstance.IsConfigured)
                        {
                            continue;
                        }

                        configurationInstance.Configure();
                    }
                }
            }
        }
    }
}
