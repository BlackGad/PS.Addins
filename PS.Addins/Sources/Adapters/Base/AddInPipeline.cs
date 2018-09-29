using System;
using PS.Addins.Host;

namespace PS.Addins.Adapters.Base
{
    public abstract class AddInPipeline : IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            OnDispose();
        }

        #endregion

        #region Members

        public abstract AddInHostSideAdapter Instantiate(AddIn addIn);

        protected virtual void OnDispose()
        {
        }

        #endregion
    }
}