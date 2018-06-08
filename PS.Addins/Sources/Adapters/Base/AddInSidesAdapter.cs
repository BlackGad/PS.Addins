using System;
using System.Reflection;
using PS.Addins.Extensions;
using PS.Addins.Host;

namespace PS.Addins.Adapters.Base
{
    public abstract class AddInSidesAdapter : IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            OnDispose();
        }

        #endregion

        #region Members

        public object HostFacadeCall(Guid instanceID, AddInHostView addInHostView, MethodInfo methodInfo, object[] args)
        {
            var result = OnHostFacadeCall(instanceID, addInHostView, methodInfo, args);
            if (methodInfo.ReturnType == typeof(void)) return null;
            return methodInfo.ReturnType.HandleBoxing(result);
        }

        public abstract Guid Instantiate(AddIn addIn);
        public abstract void Shutdown(Guid instanceId);

        protected virtual void OnDispose()
        {
        }

        protected abstract object OnHostFacadeCall(Guid instanceID, AddInHostView addInHostView, MethodInfo methodInfo, object[] args);

        #endregion
    }
}