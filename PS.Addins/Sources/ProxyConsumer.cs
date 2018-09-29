using System;
using System.Reflection;

namespace PS.Addins
{
    public class ProxyConsumer
    {
        #region Static members

        public static ProxyConsumer Create(object instance)
        {
            return new ProxyConsumer(instance);
        }

        #endregion

        private readonly object _instance;

        #region Constructors

        private ProxyConsumer(object instance)
        {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
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