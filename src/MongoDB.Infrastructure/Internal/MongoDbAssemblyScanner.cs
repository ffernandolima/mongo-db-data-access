using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MongoDB.Infrastructure.Internal
{
    internal static class MongoDbAssemblyScanner
    {
        public static Type[] Scan(IEnumerable<Assembly> scanningAssemblies, Func<Type, bool> scanningFilter)
        {
            if (scanningAssemblies is null)
            {
                throw new ArgumentNullException(nameof(scanningAssemblies), $"{nameof(scanningAssemblies)} cannot be null.");
            }

            if (scanningFilter is null)
            {
                throw new ArgumentNullException(nameof(scanningFilter), $"{nameof(scanningFilter)} cannot be null.");
            }

            var assemblyTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(scannedAssembly => scanningAssemblies.Contains(scannedAssembly))
                .SelectMany(scannedAssembly => scannedAssembly.GetTypes())
                .Where(scanningFilter)
                .ToArray();

            return assemblyTypes;
        }
    }
}
