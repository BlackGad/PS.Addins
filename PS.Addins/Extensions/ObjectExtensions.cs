using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PS.Addins.Extensions
{
    public static class ObjectExtensions
    {
        #region Static members

        public static IEnumerable<T> Enumerate<T>(this object obj)
        {
            var enumerable = obj as IEnumerable;
            return enumerable?.OfType<T>() ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> Enumerate<T>(this IEnumerable<T> obj)
        {
            var enumerable = obj as IEnumerable;
            return enumerable?.OfType<T>() ?? Enumerable.Empty<T>();
        }

        #endregion
    }
}