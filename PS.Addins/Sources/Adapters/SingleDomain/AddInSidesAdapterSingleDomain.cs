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
        private readonly Cache<Guid, object> _instances;

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
            _instances = new Cache<Guid, object>();
        }

        #endregion

        #region Override members

        protected override void OnDispose()
        {
            AppDomain.Unload(_domain);
        }

        public override Guid Instantiate(AddIn addIn)
        {
            var instanceID = Guid.NewGuid();
            //TODO: Create addin side adapter and view. Instantiate with view instance.
            //_instances.Query(instanceID, id => _domain.CreateInstanceFromAndUnwrap(addIn.AssemblyPath, addIn.AddinTypeName));
            return instanceID;
        }

        public override void Shutdown(Guid instanceId)
        {
            _instances.Remove(instanceId);
        }

        protected override object OnHostFacadeCall(Guid instanceID, AddInHostView addInHostView, MethodInfo methodInfo, object[] args)
        {
            //TODO: Send and Receive data
            if (methodInfo.ReturnType == typeof(void)) return null;
            return methodInfo.ReturnType.GetSystemDefultValue();
        }

        #endregion
    }
}