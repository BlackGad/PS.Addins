using System;
using PS.Addins.Collections;
using PS.Addins.Pipelines.Extensions;

namespace PS.Addins.Pipelines
{
    internal class DomainVault : Map<ObjectIdentifier, object>,
                                 IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            lock (Locker)
            {
                ReverseStorage.Clear();
                Storage.Clear();
            }
        }

        #endregion

        #region Nested type: DomainVaultSetter

        [Serializable]
        internal class DomainVaultSetter
        {
            private readonly string _pipelineId;

            #region Constructors

            public DomainVaultSetter(string pipelineId)
            {
                _pipelineId = pipelineId;
            }

            #endregion

            #region Members

            public void Set()
            {
                AppDomain.CurrentDomain.SetVault(_pipelineId, new DomainVault());
            }

            #endregion
        }

        #endregion
    }
}