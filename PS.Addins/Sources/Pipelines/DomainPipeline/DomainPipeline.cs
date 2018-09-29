using System;
using System.Linq;
using System.Reflection;
using PS.Addins.Pipelines.Base;

namespace PS.Addins.Pipelines
{
    public class DomainPipeline<TInstance> : IPipeline
    {
        private readonly DomainPipelineClient _client;
        private readonly AppDomain _clientDomain;
        private readonly DomainPipelineHostInvoker _host;

        #region Constructors

        public DomainPipeline() : this(null)
        {
        }

        public DomainPipeline(AppDomainSetup setup)
        {
            if (setup == null)
            {
                setup = new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
                };
            }

            _clientDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString("N"), AppDomain.CurrentDomain.Evidence, setup);
            _client = (DomainPipelineClient)_clientDomain.CreateInstanceFromAndUnwrap(typeof(DomainPipelineClient).Assembly.Location,
                                                                                      typeof(DomainPipelineClient).FullName);
            _host = new DomainPipelineHostInvoker();
            _client.Initialize(_host, typeof(TInstance).Assembly.Location, typeof(TInstance).FullName);
        }

        #endregion

        #region IPipeline Members

        public void Dispose()
        {
            AppDomain.Unload(_clientDomain);
        }

        public T Facade<T>()
        {
            return ProxyType.Create<T>(ProducerCallback);
        }

        #endregion

        #region Members

        private object ProducerCallback(MethodInfo info, object[] originalArgs)
        {
            var patchedArgs = originalArgs.ToArray();
            for (var i = 0; i < patchedArgs.Length; i++)
            {
                if (patchedArgs[i] is Delegate @delegate) patchedArgs[i] = _host.Query(@delegate);
            }

            return _client.Invoke(info.Name, info.GetParameters().Select(p => p.ParameterType).ToArray(), patchedArgs);
        }

        #endregion
    }
}