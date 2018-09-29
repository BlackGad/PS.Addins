using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PS.Addins.Extensions;
using PS.Addins.Linq.Expressions.Extensions;

namespace PS.Addins.Pipelines
{
    public class DomainPipelineClient : MarshalByRefObject,
                                        IDisposable
    {
        private readonly Cache<DomainPipelineDelegateIdentifier, Delegate> _delegatesCache;

        private DomainPipelineHostInvoker _host;
        private object _instance;
        private Type _instanceType;

        #region Constructors

        public DomainPipelineClient()
        {
            _delegatesCache = new Cache<DomainPipelineDelegateIdentifier, Delegate>();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_instance is IDisposable disposable) disposable.Dispose();
        }

        #endregion

        #region Members

        public void Initialize(DomainPipelineHostInvoker host, string assemblyPath, string typeFullName)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            var type = assembly.GetAssemblyTypes()
                               .FirstOrDefault(t => string.Equals(t.FullName,
                                                                  typeFullName,
                                                                  StringComparison.InvariantCultureIgnoreCase));
            if (type == null) throw new TypeLoadException($"Could not find '{typeFullName}' ");

            _instance = Activator.CreateInstance(type);
            _instanceType = type;
            _host = host;
        }

        public object Invoke(string methodName, IEnumerable<Type> methodParameters, object[] originalArgs)
        {
            var method = _instanceType.GetSimilarMethod(methodName, methodParameters.ToArray());

            var patchedArgs = originalArgs.ToArray();
            for (var i = 0; i < originalArgs.Length; i++)
            {
                var arg = patchedArgs[i];
                if (arg is DomainPipelineDelegateIdentifier identifier)
                {
                    patchedArgs[i] = _delegatesCache.Query(identifier, id => CreateProxyDelegate(id.DelegateType, id));
                }
            }

            return method.Invoke(_instance, patchedArgs);
        }

        private Delegate CreateProxyDelegate(Type delegateType, DomainPipelineDelegateIdentifier identifier)
        {
            if (delegateType == null) throw new ArgumentNullException(nameof(delegateType));
            var method = delegateType.GetMethod("Invoke");
            if (method == null) throw new MissingMethodException();

            var parameterExpressions = method.GetParameters()
                                             .Select(p => Expression.Parameter(p.ParameterType))
                                             .ToList();

            //Call Host.Invoke method with delegate identifier and real arguments
            var body = new List<Expression>
            {
                Expression.Call(Expression.Constant(_host),
                                DomainPipelineHostInvoker.DomainPipelineHostInvokerInvokeMethod,
                                Expression.Constant(identifier),
                                Expression.NewArrayInit(typeof(object), parameterExpressions))
            };

            var lambdaResult = Expression.Lambda(delegateType, body.PackExpressions(), parameterExpressions);
            return lambdaResult.Compile();
        }

        #endregion
    }
}