using System;
using PS.Addins.Adapters.Base;

namespace PS.Addins.Host
{
    public class AddInInstance : IDisposable
    {
        private readonly AddIn _addIn;
        private readonly AddInHostSideAdapter _addInHostSideAdapter;
        private readonly Cache<Type, object> _contractFacadeCache;

        #region Constructors

        internal AddInInstance(AddIn addIn, AddInHostSideAdapter addInHostSideAdapter)
        {
            if (addIn == null) throw new ArgumentNullException(nameof(addIn));
            if (addInHostSideAdapter == null) throw new ArgumentNullException(nameof(addInHostSideAdapter));

            _addIn = addIn;
            _addInHostSideAdapter = addInHostSideAdapter;
            _contractFacadeCache = new Cache<Type, object>();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _addInHostSideAdapter.Dispose();
        }

        #endregion

        #region Members

        public T Contract<T>()
        {
            return (T)_contractFacadeCache.Query(typeof(T), CreateContractFacade);
        }

        private object CreateContractFacade(Type contractType)
        {
            return ProxyProducer.Create(contractType, (method, args) => _addInHostSideAdapter.Call(contractType, method, args));
        }

        #endregion
    }
}