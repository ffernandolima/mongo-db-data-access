using System;
using System.Collections.Generic;
using System.Reflection;

namespace MongoDB.Infrastructure
{
    public class MongoDbFluentConfigurationOptions
    {
        public bool EnableAssemblyScanning { get; set; } = true;
        public IEnumerable<Assembly> ScanningAssemblies { get; set; }
            = new Assembly[] { Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly() };
        public Func<Type, bool> ScanningFilter { get; set; }
    }
}
