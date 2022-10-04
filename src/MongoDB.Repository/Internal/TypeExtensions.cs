using System;

namespace MongoDB.Repository.Internal
{
    internal static class TypeExtensions
    {
        public static bool IsGenericType(this Type sourceType, Type targetType) => sourceType.IsGenericType(targetType, out _);

        public static bool IsGenericType(this Type sourceType, Type targetType, out Type[] sourceArguments)
        {
            if (sourceType is null)
            {
                throw new ArgumentNullException(nameof(sourceType), $"{nameof(sourceType)} cannot be null.");
            }

            if (targetType is null)
            {
                throw new ArgumentNullException(nameof(targetType), $"{nameof(targetType)} cannot be null.");
            }

            if (!sourceType.IsGenericType)
            {
                sourceArguments = null;

                return false;
            }

            var typeDefinition = sourceType.GetGenericTypeDefinition();

            if (typeDefinition != targetType)
            {
                sourceArguments = null;

                return false;
            }

            sourceArguments = sourceType.GenericTypeArguments;

            return true;
        }
    }
}
