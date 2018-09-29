using System;
using Contracts.HostSide;
using PS.Addins.Pipelines;

namespace Host
{
    class Program
    {
        #region Static members

        static void Main()
        {
            try
            {
                using (var pipeline = new DirectPipeline<Instance>())
                {
                    var facade = pipeline.Facade<IHostViewContract>();
                    void EventHandler(object sender, EventArgs eventArgs)
                    {
                    }

                    Console.WriteLine("Attaching event handler...");
                    facade.Event += EventHandler;
                    Console.WriteLine("Done.");

                    Console.WriteLine("Setting property...");
                    facade.Property = 1;
                    Console.WriteLine("Done.");

                    Console.WriteLine("Setting indexer...");
                    facade[0] = 1;
                    Console.WriteLine("Done.");

                    Console.WriteLine("Function result: " + facade.Function(2, "Hello"));
                    Console.WriteLine("Property result: " + facade.Property);
                    Console.WriteLine("Indexer result: " + facade[0]);

                    Console.WriteLine("Detaching event handler...");
                    facade.Event -= EventHandler;
                    Console.WriteLine("Done.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException());
            }

            Console.ReadLine();
        }

        #endregion
    }

    /*public class SeparateDomainPipeline
    {
        private readonly AppDomain _domain;
        private readonly Cache<AddInHostSideAdapter, object> _instances;

        #region Constructors

        public AddInPipelineSingleDomain() : this(null)
        {
        }

        public AddInPipelineSingleDomain(AppDomainSetup setup)
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
            return methodInfo.ReturnType.GetSystemDefaultValue();
        }

        private void Shutdown(AddInHostSideAdapter instanceId)
        {
            _instances.Remove(instanceId);
        }

        #endregion
    }*/
}