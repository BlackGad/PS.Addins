using System;
using PS.Addins.Pipelines.Base;

namespace PS.Addins.Pipelines
{
    class DomainPipeline : IPipeline
    {
        private readonly AppDomain _domain;

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

            _domain = AppDomain.CreateDomain(Guid.NewGuid().ToString("N"), AppDomain.CurrentDomain.Evidence, setup);
        }

        #endregion

        #region IPipeline Members

        public void Dispose()
        {
            AppDomain.Unload(_domain);
        }

        public T Facade<T>()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}