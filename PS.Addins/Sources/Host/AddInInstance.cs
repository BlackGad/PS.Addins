using System;
using System.Globalization;
using System.Reflection;
using PS.Addins.Extensions;

namespace PS.Addins.Host
{
    public class AddInInstance : IDisposable
    {
        #region Constants

        private static readonly Cache<Type, AddInHostView> HostProxyTypesCache;

        #endregion

        private readonly Cache<Type, object> _contractFacadeCache;

        #region Constructors

        static AddInInstance()
        {
            HostProxyTypesCache = new Cache<Type, AddInHostView>();
        }

        internal AddInInstance()
        {
            _contractFacadeCache = new Cache<Type, object>();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region Members

        public T Contract<T>()
        {
            return (T)_contractFacadeCache.Query(typeof(T), CreateContractFacade);
        }

        private object CreateContractFacade(Type contractType)
        {
            var addInHostView = HostProxyTypesCache.Query(contractType, key => new AddInHostView(key));
            var callBack = new Func<string, object[], object>((id, args) => ProxyCallback(addInHostView, addInHostView.ContractMethodsMap[id], args));
            return Activator.CreateInstance(addInHostView.HostSideAdapterType,
                                            BindingFlags.Instance | BindingFlags.Public,
                                            null,
                                            new object[] { callBack },
                                            CultureInfo.CurrentCulture);
        }

        private object ProxyCallback(AddInHostView addInHostView, MethodInfo methodInfo, object[] args)
        {
            //TODO: HSAdapter here

            if (methodInfo.ReturnType == typeof(void)) return null;
            var result = methodInfo.ReturnType.GetSystemDefultValue();
            return methodInfo.ReturnType.HandleBoxing(result);
        }

        #endregion
    }
}