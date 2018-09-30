using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PS.Addins.Extensions;
using PS.Addins.Linq.Expressions.Extensions;
using PS.Addins.Pipelines.Extensions;

namespace PS.Addins.Pipelines
{
    class DomainInvoker : MarshalByRefObject
    {
        #region Constants

        private static readonly MethodInfo DomainPipelinePatchArgsMethod;

        private static readonly MethodInfo InvokeDelegateMethod;

        #endregion

        private readonly string _pipelineId;

        private DomainVault _vault;

        #region Constructors

        static DomainInvoker()
        {
            InvokeDelegateMethod = typeof(DomainInvoker).GetMethod(nameof(InvokeDelegate));
            DomainPipelinePatchArgsMethod = typeof(DomainPipeline).GetMethod(nameof(DomainPipeline.PackArgs));
        }

        public DomainInvoker(string pipelineId)
        {
            _pipelineId = pipelineId ?? throw new ArgumentNullException(nameof(pipelineId));
        }

        #endregion

        #region Members

        public ObjectIdentifier CreateDelegateReflex(Type delegateType)
        {
            if (delegateType == null) throw new ArgumentNullException(nameof(delegateType));

            var method = delegateType.GetMethod("Invoke");
            if (method == null) throw new MissingMethodException();

            var identifier = new ObjectIdentifier(_pipelineId);

            //Call RemoteInvoker.Invoke method with delegate identifier and real arguments
            var invoker = AppDomain.CurrentDomain.GetRemoteInvoker(_pipelineId);

            var parameterExpressions = method.GetParameters()
                                             .Select(p => Expression.Parameter(p.ParameterType))
                                             .ToList();

            var parameterTypeExpressions = method.GetParameters()
                                                 .Select(p => Expression.Constant(p.ParameterType))
                                                 .ToList();

            var patchedArgsExpression = Expression.Call(DomainPipelinePatchArgsMethod,
                                                        Expression.Constant(_pipelineId),
                                                        Expression.NewArrayInit(typeof(Type), parameterTypeExpressions),
                                                        Expression.NewArrayInit(typeof(object), parameterExpressions));

            var body = new List<Expression>
            {
                Expression.Call(Expression.Constant(invoker),
                                InvokeDelegateMethod,
                                Expression.Constant(identifier),
                                patchedArgsExpression)
            };

            var lambdaResult = Expression.Lambda(delegateType, body.PackExpressions(), parameterExpressions);
            var @delegate = lambdaResult.Compile();

            QueryVault().Register(identifier, @delegate);

            return identifier;
        }

        public ObjectIdentifier CreateObject(string assemblyLocation, string typeName)
        {
            var assembly = Assembly.LoadFrom(assemblyLocation);
            var type = assembly.GetAssemblyTypes()
                               .FirstOrDefault(t => string.Equals(t.FullName,
                                                                  typeName,
                                                                  StringComparison.InvariantCultureIgnoreCase));
            if (type == null) throw new TypeLoadException($"Could not find '{typeName}' in {assemblyLocation}");

            var @object = Activator.CreateInstance(type);
            var identifier = new ObjectIdentifier(_pipelineId);

            QueryVault().Register(identifier, @object);
            return identifier;
        }

        public ObjectIdentifier CreateObjectReflex(Type objectType)
        {
            var identifier = new ObjectIdentifier(_pipelineId);
            var reflex = DomainPipeline.CreateReflexForObject(identifier, objectType);
            QueryVault().Register(identifier, reflex);
            return identifier;
        }

        public object InvokeDelegate(ObjectIdentifier identifier, object[] args)
        {
            var @delegate = (Delegate)QueryVault().Query(identifier);
            var result = @delegate.DynamicInvoke(DomainPipeline.UnpackArgs(identifier.PipelineId, args));
            return DomainPipeline.PackArgs(identifier.PipelineId, new[] { @delegate.Method.ReturnType }, new[] { result }).First();
        }

        public object InvokeObjectMethod(ObjectIdentifier identifier, string methodName, IEnumerable<Type> methodParameters, object[] args)
        {
            var vault = QueryVault();

            var instance = vault.Query(identifier);
            var instanceType = instance.GetType();

            var method = instanceType.GetSimilarMethod(methodName, methodParameters.ToArray());
            var result = method.Invoke(instance, DomainPipeline.UnpackArgs(identifier.PipelineId, args));
            return DomainPipeline.PackArgs(identifier.PipelineId, new[] { method.ReturnType }, new[] { result }).First();
        }

        private DomainVault QueryVault()
        {
            return _vault ?? (_vault = AppDomain.CurrentDomain.GetVault(_pipelineId));
        }

        #endregion
    }
}