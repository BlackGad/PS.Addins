using System;

namespace PS.Addins.Pipelines.Base
{
    public interface IPipeline : IDisposable
    {
        #region Members

        void Dispose();
        T Facade<T>();

        #endregion
    }
}