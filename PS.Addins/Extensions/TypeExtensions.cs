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
        public static object GetSystemDefultValue(this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        #endregion
    }
}