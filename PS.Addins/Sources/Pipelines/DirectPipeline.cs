using System;
using System.Linq;
using System.Reflection;
using PS.Addins.Pipelines.Base;

namespace PS.Addins.Pipelines
{
    public class DirectPipeline<TInstance> : IDisposable, IPipeline
    {
        private readonly ProxyConsumer _consumer;

        #region Constructors

        public DirectPipeline()
        {
            _consumer = new ProxyConsumer(typeof(TInstance));
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _consumer.Dispose();
        }

        #endregion

        #region Members

        public T Facade<T>()
        {
            return ProxyProducer.Create<T>(ProducerCallback);
        }

        private object ProducerCallback(MethodInfo info, object[] args)
        {
            if (info.DeclaringType?.IsAssignableFrom(_consumer.InstanceType) != true)
            {
                var methodParams = info.GetParameters().Select(p => p.ParameterType).ToArray();
                var consumerMethod = _consumer.InstanceType
                                              .GetMethod(info.Name,
                                                         BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                         null,
                                                         methodParams,
                                                         null);
                info = consumerMethod ?? throw new InvalidCastException();
            }

            return _consumer.Consume(info, args);
        }

        #endregion
    }
}