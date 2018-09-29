using System;
using System.Reflection;

namespace PS.Addins
{
    public class ProxyConsumer : IDisposable
    {
        private readonly object _instance;

        #region Constructors

        public ProxyConsumer(Type instanceType)
        {
            InstanceType = instanceType ?? throw new ArgumentNullException(nameof(instanceType));
            _instance = Activator.CreateInstance(instanceType);
        }

        #endregion

        #region Properties

        public Type InstanceType { get; }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_instance is IDisposable disposable) disposable.Dispose();
        }

        #endregion

        #region Members

        public object Consume(MethodInfo method, object[] args)
        {
            return method.Invoke(_instance, args);
        }

        #endregion
    }
}