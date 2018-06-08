using System;
using System.Reflection;

namespace PS.Addins.Adapters.Base
{
    public class AddInHostSideAdapterDelegate : AddInHostSideAdapter
    {
        private readonly Func<AddInHostSideAdapter, Type, MethodInfo, object[], object> _callFunc;
        private readonly Action<AddInHostSideAdapter> _disposeAction;

        #region Constructors

        public AddInHostSideAdapterDelegate(Func<AddInHostSideAdapter, Type, MethodInfo, object[], object> callFunc,
                                            Action<AddInHostSideAdapter> disposeAction)
        {
            _callFunc = callFunc;
            _disposeAction = disposeAction;
        }

        #endregion

        #region Override members

        public override void Dispose()
        {
            _disposeAction?.Invoke(this);
        }

        public override object Call(Type contractType, MethodInfo methodInfo, object[] args)
        {
            return _callFunc?.Invoke(this, contractType, methodInfo, args);
        }

        #endregion
    }
}