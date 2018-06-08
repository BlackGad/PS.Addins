using System;
using System.Reflection;

namespace PS.Addins.Host
{
    public class AddInHostViewAggregation
    {
        private readonly AddInHostView _addInHostView;
        private readonly Func<AddInHostView, MethodInfo, object[], object> _callback;

        #region Constructors

        internal AddInHostViewAggregation(AddInHostView addInHostView, Func<AddInHostView, MethodInfo, object[], object> callback)
        {
            if (addInHostView == null) throw new ArgumentNullException(nameof(addInHostView));
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            _addInHostView = addInHostView;
            _callback = callback;
        }

        #endregion

        #region Members

        public object CallMethod(string id, object[] args)
        {
            var methodInfo = _addInHostView.ContractMethodsMap[id];
            var result = _callback(_addInHostView, methodInfo, args);
            if (methodInfo.ReturnType == typeof(void)) return null;
            return HandleBoxing(methodInfo.ReturnType, result);
        }

        private object HandleBoxing(Type targetType, object value)
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