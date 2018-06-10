using System;
using System.Reflection;

namespace Contracts.Contract
{
    public interface IContract
    {
        #region Members

        object AddinHostCall(Type contractType, MethodInfo methodInfo, object[] args);

        object HostAddinCall(Type contractType, MethodInfo methodInfo, object[] args);

        #endregion
    }
}