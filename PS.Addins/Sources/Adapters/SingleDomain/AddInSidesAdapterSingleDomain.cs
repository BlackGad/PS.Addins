using System;
using System.Reflection;
using PS.Addins.Adapters.Base;
using PS.Addins.Extensions;
using PS.Addins.Host;

namespace PS.Addins.Adapters.SingleDomain
{
    public class AddInSidesAdapterSingleDomain : AddInSidesAdapter
    {
        private readonly AppDomain _domain;
        private readonly Cache<AddInHostSideAdapter, object> _instances;

        #region Constructors

        public AddInSidesAdapterSingleDomain() : this(null)
        {
        }

        public AddInSidesAdapterSingleDomain(AppDomainSetup setup)
        {
            if (setup == null)
            {
                setup = new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
                };
            }
            _domain = AppDomain.CreateDomain(Guid.NewGuid().ToString("N"), AppDomain.CurrentDomain.Evidence, setup);
            _instances = new Cache<AddInHostSideAdapter, object>();
        }

        #endregion

        #region Override members

        protected override void OnDispose()
        {
            AppDomain.Unload(_domain);
        }

        public override AddInHostSideAdapter Instantiate(AddIn addIn)
        {
            var result = new AddInHostSideAdapterDelegate(OnHostFacadeCall, Shutdown);

            //TODO: Create addin side adapter and view. Instantiate with view instance.
            //_instances.Query(result, id => AddIn side adapter);
            return result;
        }

        #endregion

        #region Members

        private object OnHostFacadeCall(AddInHostSideAdapter addInHostSideAdapter, Type contractType, MethodInfo methodInfo, object[] args)
        {
            //TODO: Send and Receive data
            if (methodInfo.ReturnType == typeof(void)) return null;
            return methodInfo.ReturnType.GetSystemDefultValue();
        }

        private void Shutdown(AddInHostSideAdapter instanceId)
        {
            _instances.Remove(instanceId);
        }

        #endregion
    }
}