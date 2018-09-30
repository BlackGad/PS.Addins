using System;

namespace PS.Addins.Pipelines.Base
{
    public interface IPipeline : IDisposable
    {
        #region Members

        T CreateObject<T>(string assemblyLocation, string typeName);

        #endregion
    }
}