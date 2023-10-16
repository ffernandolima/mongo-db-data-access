using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Infrastructure.Internal
{
    internal static class MongoDbTypeExtensions
    {
        private static readonly Dictionary<Type, string> _builtInTypeNames = new()
        {
            { typeof(void),    "void"    },
            { typeof(bool),    "bool"    },
            { typeof(byte),    "byte"    },
            { typeof(char),    "char"    },
            { typeof(decimal), "decimal" },
            { typeof(double),  "double"  },
            { typeof(float),   "float"   },
            { typeof(int),     "int"     },
            { typeof(long),    "long"    },
            { typeof(object),  "object"  },
            { typeof(sbyte),   "sbyte"   },
            { typeof(short),   "short"   },
            { typeof(string),  "string"  },
            { typeof(uint),    "uint"    },
            { typeof(ulong),   "ulong"   },
            { typeof(ushort),  "ushort"  }
        };

        public static string ExtractTypeName(this Type sourceType)
        {
            if (sourceType.IsGenericType)
            {
                var index = sourceType.Name.IndexOf('`');
                if (index > -1)
                {
                    var genericArguments = string.Join(", ", sourceType.GetGenericArguments()
                        .Select(ExtractTypeName));

                    var genericTypeName = $"{sourceType.Name.Remove(index)}<{genericArguments}>";

                    return genericTypeName;
                }
            }

            if (_builtInTypeNames.TryGetValue(sourceType, out var builtInTypeName))
            {
                return builtInTypeName;
            }

            return sourceType.Name;
        }
    }
}
