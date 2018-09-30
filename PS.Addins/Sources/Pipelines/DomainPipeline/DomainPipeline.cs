using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PS.Addins.Pipelines.Base;
using PS.Addins.Pipelines.Extensions;

namespace PS.Addins.Pipelines
{
    public class DomainPipeline : IPipeline
    {
        #region Static members

        public static object[] PackArgs(string pipelineId, Type[] parameters, IEnumerable<object> args)
        {
            var invoker = AppDomain.CurrentDomain.GetRemoteInvoker(pipelineId);
            if (invoker == null) throw new InvalidOperationException("Domain was not configured for remoting");

            var vault = AppDomain.CurrentDomain.GetVault(pipelineId);
            if (vault == null) throw new InvalidOperationException("Domain was not configured for remoting");

            var patchedArgs = args.ToArray();
            for (var i = 0; i < patchedArgs.Length; i++)
            {
                if (patchedArgs[i] == null) continue;

                var argumentIdentifier = vault.Query(patchedArgs[i]);
                if (argumentIdentifier != null)
                {
                    patchedArgs[i] = argumentIdentifier;
                    continue;
                }

                if (patchedArgs[i] is Delegate @delegate)
                {
                    argumentIdentifier = CreateReflexForDelegate(pipelineId, @delegate.GetType());
                    vault.Register(argumentIdentifier, @delegate);

                    patchedArgs[i] = argumentIdentifier;
                }
                else
                {
                    var argumentType = parameters[i];
                    if (argumentType.IsMarshalByRef == false && argumentType.IsSerializable == false && argumentType.IsInterface)
                    {
                        argumentIdentifier = invoker.CreateObjectReflex(argumentType);
                        vault.Register(argumentIdentifier, patchedArgs[i]);

                        patchedArgs[i] = argumentIdentifier;
                    }
                }
            }

            return patchedArgs;
        }

        public static object[] UnpackArgs(string pipelineId, IEnumerable<object> args)
        {
            var vault = AppDomain.CurrentDomain.GetVault(pipelineId);
            if (vault == null) throw new InvalidOperationException("Domain was not configured for remoting");

            var resolvedArgs = args.ToArray();
            for (var i = 0; i < resolvedArgs.Length; i++)
            {
                if (resolvedArgs[i] is ObjectIdentifier objectIdentifier)
                {
                    resolvedArgs[i] = vault.Query(objectIdentifier);
                }
            }

            return resolvedArgs;
        }

        internal static ObjectIdentifier CreateReflexForDelegate(string pipelineId, Type delegateType)
        {
            if (pipelineId == null) throw new ArgumentNullException(nameof(pipelineId));

            var invoker = AppDomain.CurrentDomain.GetRemoteInvoker(pipelineId);
            if (invoker == null) throw new InvalidOperationException("Domain was not configured for remoting");

            return invoker.CreateDelegateReflex(delegateType);
        }

        internal static object CreateReflexForObject(ObjectIdentifier identifier, Type facadeType)
        {
            if (facadeType == null) throw new ArgumentNullException(nameof(facadeType));
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            return ProxyType.Create(facadeType, (info, args) => FacadeCallback(identifier, info, args));
        }

        internal static ObjectIdentifier CreateRemoteObject(string pipelineId, string assemblyLocation, string typeName)
        {
            if (pipelineId == null) throw new ArgumentNullException(nameof(pipelineId));
            if (assemblyLocation == null) throw new ArgumentNullException(nameof(assemblyLocation));
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));

            var invoker = AppDomain.CurrentDomain.GetRemoteInvoker(pipelineId);
            if (invoker == null) throw new InvalidOperationException("Domain was not configured for remoting");

            return invoker.CreateObject(assemblyLocation, typeName);
        }

        private static object FacadeCallback(ObjectIdentifier identifier, MethodInfo info, object[] args)
        {
            var invoker = AppDomain.CurrentDomain.GetRemoteInvoker(identifier.PipelineId);
            if (invoker == null) throw new InvalidOperationException("Domain was not configured for remoting");

            var parameters = info.GetParameters().Select(p => p.ParameterType).ToArray();
            var result = invoker.InvokeObjectMethod(identifier,
                                                    info.Name,
                                                    parameters,
                                                    PackArgs(identifier.PipelineId, parameters, args));
            return UnpackArgs(identifier.PipelineId, new[] { result }).First();
        }

        private static void SetupPipelineDomains(string pipelineId, AppDomain host, AppDomain remote)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (remote == null) throw new ArgumentNullException(nameof(remote));

            var domainInvokerType = typeof(DomainInvoker);
            if (domainInvokerType.FullName == null) throw new InvalidOperationException();

            var setter = new DomainVault.DomainVaultSetter(pipelineId);

            host.SetRemoteInvoker(pipelineId, remote.CreateObject<DomainInvoker>(pipelineId));
            host.DoCallBack(setter.Set);

            remote.SetRemoteInvoker(pipelineId, host.CreateObject<DomainInvoker>(pipelineId));
            remote.DoCallBack(setter.Set);
        }

        #endregion

        private readonly string _pipelineId;
        private readonly AppDomain _remoteDomain;

        private bool _isDisposed;

        #region Constructors

        public DomainPipeline(AppDomainSetup setup = null)
        {
            if (setup == null)
            {
                setup = new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
                };
            }

            _pipelineId = Guid.NewGuid().ToString("N");
            _remoteDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString("N"), AppDomain.CurrentDomain.Evidence, setup);

            SetupPipelineDomains(_pipelineId, AppDomain.CurrentDomain, _remoteDomain);
        }

        #endregion

        #region IPipeline Members

        public void Dispose()
        {
            lock (this)
            {
                if (_isDisposed) return;

                AppDomain.CurrentDomain.GetVault(_pipelineId)?.Dispose();

                AppDomain.CurrentDomain.SetVault(_pipelineId, null);
                AppDomain.CurrentDomain.SetRemoteInvoker(_pipelineId, null);

                AppDomain.Unload(_remoteDomain);

                _isDisposed = true;
            }
        }

        public T CreateObject<T>(string assemblyLocation, string typeName)
        {
            lock (this)
            {
                if (_isDisposed) throw new ObjectDisposedException("Pipeline is disposed");

                if (!typeof(T).IsInterface) throw new ArgumentException("Facade type must be an interface");
                var identifier = CreateRemoteObject(_pipelineId, assemblyLocation, typeName);
                var reflexForObject = CreateReflexForObject(identifier, typeof(T));

                var vault = AppDomain.CurrentDomain.GetVault(_pipelineId);
                vault.Register(identifier, reflexForObject);

                return (T)reflexForObject;
            }
        }

        #endregion
    }
}