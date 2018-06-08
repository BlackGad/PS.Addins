using System;
using System.Globalization;
using System.Reflection;
using PS.Addins.Adapters.Base;

namespace PS.Addins.Host
{
    public class AddInInstance : IDisposable
    {
        #region Constants

        private static readonly Cache<Type, AddInHostView> AddInHostViewFacadeTypesCache;

        #endregion

        private readonly AddIn _addIn;

        private readonly Cache<Type, object> _contractFacadeCache;
        private readonly Guid _instanceID;
        private readonly AddInSidesAdapter _sidesAdapter;

        #region Constructors

        static AddInInstance()
        {
            AddInHostViewFacadeTypesCache = new Cache<Type, AddInHostView>();
        }

        internal AddInInstance(AddIn addIn, AddInSidesAdapter sidesAdapter)
        {
            if (addIn == null) throw new ArgumentNullException(nameof(addIn));
            if (sidesAdapter == null) throw new ArgumentNullException(nameof(sidesAdapter));

            _instanceID = sidesAdapter.Instantiate(addIn);
            _addIn = addIn;
            _sidesAdapter = sidesAdapter;
            _contractFacadeCache = new Cache<Type, object>();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _sidesAdapter.Shutdown(_instanceID);
        }

        #endregion

        #region Members

        public T Contract<T>()
        {
            return (T)_contractFacadeCache.Query(typeof(T), CreateContractFacade);
        }

        private object CreateContractFacade(Type contractType)
        {
            var addInHostView = AddInHostViewFacadeTypesCache.Query(contractType, key => new AddInHostView(key));
            var callBack = new Func<string, object[], object>((id, args) => _sidesAdapter.HostFacadeCall(_instanceID,
                                                                                                         addInHostView,
                                                                                                         addInHostView.ContractMethodsMap[id],
                                                                                                         args));
            return Activator.CreateInstance(addInHostView.AddInHostViewProxyType,
                                            BindingFlags.Instance | BindingFlags.Public,
                                            null,
                                            new object[] { callBack },
                                            CultureInfo.CurrentCulture);
        }

        #endregion
    }
}