using System;
using System.Reflection;
using PS.Addins.Extensions;
using PS.Addins.Pipelines.Base;

namespace PS.Addins.Pipelines
{
    public class DirectPipeline<TInstance> : IDisposable,
                                             IPipeline
    {
        private readonly TInstance _instance;

        #region Constructors

        public DirectPipeline()
        {
            _instance = Activator.CreateInstance<TInstance>();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_instance is IDisposable disposable) disposable.Dispose();
        }

        #endregion

        #region IPipeline Members

        public T Facade<T>()
        {
            return ProxyType.Create<T>(ProducerCallback);
        }

        #endregion

        #region Members

        private object ProducerCallback(MethodInfo method, object[] args)
        {
            return _instance.GetType()
                            .GetSimilarMethod(method)
                            .Invoke(_instance, args);
        }

        #endregion
    }
}