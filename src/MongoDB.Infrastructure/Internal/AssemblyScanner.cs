using System;
using System.Linq;
using System.Reflection;

namespace MongoDB.Infrastructure.Internal
{
    internal static class AssemblyScanner
    {
        public static Type[] Scan(Assembly targetAssembly, Func<Type, bool> scanFilter)
        {
            if (targetAssembly == null)
            {
                throw new ArgumentNullException(nameof(targetAssembly));
            }

            if (scanFilter == null)
            {
                throw new ArgumentNullException(nameof(scanFilter));
            }

            var assemblyTypes = AppDomain.CurrentDomain.GetAssemblies()
                                                       .Where(scannedAssembly => scannedAssembly == targetAssembly)
                                                       .SelectMany(scannedAssembly => scannedAssembly.GetTypes())
                                                       .Where(scanFilter)
                                                       .ToArray();
            return assemblyTypes;
        }
    }
}
