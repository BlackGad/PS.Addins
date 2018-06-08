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

        private readonly Cache<Type, object> _facadeCache;

        #region Constructors

        static AddInInstance()
        {
            HostProxyTypesCache = new Cache<Type, AddInHostView>();
        }

        public AddInInstance()
        {
            _facadeCache = new Cache<Type, object>();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region Members

        public T Facade<T>()
        {
            return (T)_facadeCache.Query(typeof(T), CreateFacade);
        }

        private object CreateFacade(Type contractType)
        {
            var descriptor = HostProxyTypesCache.Query(contractType, key => new AddInHostView(key));
            var proxyAggregation = new AddInHostViewAggregation(descriptor, ProxyCallback);
            return Activator.CreateInstance(descriptor.HostSideAdapterType,
                                            BindingFlags.Instance | BindingFlags.Public,
                                            null,
                                            new object[] { proxyAggregation },
                                            CultureInfo.CurrentCulture);
        }

        private object ProxyCallback(AddInHostView addInHostView, MethodInfo methodInfo, object[] args)
        {
            //TODO: HSAdapter here


            if (methodInfo.ReturnType == typeof(void)) return null;
            var result = methodInfo.ReturnType.GetSystemDefultValue();
            return result;
        }

        #endregion
    }
}