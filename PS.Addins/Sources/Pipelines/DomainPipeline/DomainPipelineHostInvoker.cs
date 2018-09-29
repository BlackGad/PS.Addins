using System;
using System.Collections.Generic;
using System.Reflection;

namespace PS.Addins.Pipelines
{
    public class DomainPipelineHostInvoker : MarshalByRefObject
    {
        #region Constants

        public static readonly MethodInfo DomainPipelineHostInvokerInvokeMethod;

        #endregion

        private readonly Dictionary<Delegate, DomainPipelineDelegateIdentifier> _backMap;
        private readonly object _locker;
        private readonly Dictionary<DomainPipelineDelegateIdentifier, Delegate> _map;

        #region Constructors

        static DomainPipelineHostInvoker()
        {
            var type = typeof(DomainPipelineHostInvoker);
            DomainPipelineHostInvokerInvokeMethod = type.GetMethod(nameof(Invoke), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public DomainPipelineHostInvoker()
        {
            _backMap = new Dictionary<Delegate, DomainPipelineDelegateIdentifier>();
            _map = new Dictionary<DomainPipelineDelegateIdentifier, Delegate>();
            _locker = new object();
        }

        #endregion

        #region Members

        internal object Invoke(DomainPipelineDelegateIdentifier identifier, object[] args)
        {
            Delegate @delegate;
            lock (_locker)
            {
                if (!_map.ContainsKey(identifier)) throw new InvalidOperationException();
                @delegate = _map[identifier];
            }

            return @delegate.DynamicInvoke(args);
        }

        internal DomainPipelineDelegateIdentifier Query(Delegate @delegate)
        {
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));
            lock (_locker)
            {
                if (_backMap.ContainsKey(@delegate))
                {
                    return _backMap[@delegate];
                }

                var identifier = new DomainPipelineDelegateIdentifier(@delegate.GetType());
                _backMap.Add(@delegate, identifier);
                _map.Add(identifier, @delegate);
                return identifier;
            }
        }

        #endregion
    }
}