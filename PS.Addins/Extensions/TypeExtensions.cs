using System;
using System.Linq;
using System.Reflection;

namespace PS.Addins.Extensions
{
    public static class TypeExtensions
    {
        #region Static members

        public static Type[] GetAssemblyTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types;
            }
        }

        public static MethodInfo GetSimilarMethod(this Type targetType, string methodName, Type[] parameters)
        {
            var consumerMethod = targetType.GetMethod(methodName,
                                                      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                      null,
                                                      parameters,
                                                      null);
            return consumerMethod ?? throw new InvalidCastException();
        }

        public static MethodInfo GetSimilarMethod(this Type targetType, MethodInfo method)
        {
            if (method.DeclaringType?.IsAssignableFrom(targetType) == true) return method;

            var methodParams = method.GetParameters().Select(p => p.ParameterType).ToArray();
            var consumerMethod = targetType.GetMethod(method.Name,
                                                      BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                      null,
                                                      methodParams,
                                                      null);
            return consumerMethod ?? throw new InvalidCastException();
        }

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