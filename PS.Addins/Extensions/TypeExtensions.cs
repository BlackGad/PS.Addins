using System;

namespace PS.Addins.Extensions
{
    public static class TypeExtensions
    {
        #region Static members

        /// <summary>
        ///     Gets type system default value. Default instance for value types, null for reference types
        /// </summary>
        /// <param name="type">Given type.</param>
        /// <returns>Default type value.</returns>
        public static object GetSystemDefaultValue(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static object HandleBoxing(this Type targetType, object value)
        {
            if (value?.GetType() == targetType) return value;

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>) && value != null)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                return Convert.ChangeType(value, Nullable.GetUnderlyingType(targetType));
            }

            if (targetType.IsValueType) return Convert.ChangeType(value, targetType);
            return value;
        }

        #endregion
    }
}