using System;
using System.Reflection;

namespace PS.Addins.Adapters.Base
{
    public abstract class AddInHostSideAdapter : IDisposable
    {
        #region IDisposable Members

        public abstract void Dispose();

        #endregion

        #region Members

        public abstract object Call(Type contractType, MethodInfo methodInfo, object[] args);

        #endregion
    }
}