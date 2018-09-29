using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PS.Addins
{
    public class ProxyType
    {
        #region Constants

        private static readonly Cache<Type, ProxyType> Cache;

        private static readonly MethodInfo CallBackInvokeMethodInfo;
        private static readonly Type CallBackType;
        private static readonly ConstructorInfo DefaultObjectConstructorInfo;

        #endregion

        #region Static members

        public static T Create<T>(Func<MethodInfo, object[], object> callBack)
        {
            return (T)Create(typeof(T), callBack);
        }

        public static object Create(Type type, Func<MethodInfo, object[], object> callBack)
        {
            if (!type.IsInterface) throw new NotSupportedException($"{type} must be interface type");

            var producer = Cache.Query(type, t => new ProxyType(t));

            var internalCallBack = new Func<string, object[], object>((id, args) => callBack?.Invoke(producer.MethodsMap[id], args));
            return Activator.CreateInstance(producer.ProducerType,
                                            BindingFlags.Instance | BindingFlags.Public,
                                            null,
                                            new object[] { internalCallBack },
                                            CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// </summary>
        private static Type GenerateMockType(Type interfaceType, Dictionary<MethodInfo, string> map)
        {
            var assemblyName = new AssemblyName($"{interfaceType.Name}_{Guid.NewGuid():N}");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("module");

            var proxyTypeName = $"{interfaceType.Name}_{Guid.NewGuid():N}";
            var typeBuilder = moduleBuilder.DefineType(proxyTypeName);
            typeBuilder.AddInterfaceImplementation(interfaceType);

            var callbackFieldBuilder = typeBuilder.DefineField("_callback", CallBackType, FieldAttributes.Private);

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
                                                                   CallingConventions.Standard,
                                                                   new[] { CallBackType });

            var constructorIL = constructorBuilder.GetILGenerator();
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Call, DefaultObjectConstructorInfo);
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Ldarg_1);
            constructorIL.Emit(OpCodes.Stfld, callbackFieldBuilder);
            constructorIL.Emit(OpCodes.Ret);

            var methodMap = new Dictionary<string, MethodBuilder>();
            var contractMethods = interfaceType.GetMethods();
            foreach (var method in contractMethods)
            {
                var attributes = method.Attributes;
                attributes &= ~MethodAttributes.Abstract;
                var returnType = method.ReturnType;
                var parameters = method.GetParameters();
                var methodBuilder = typeBuilder.DefineMethod(method.Name,
                                                             attributes,
                                                             method.CallingConvention,
                                                             returnType,
                                                             parameters.Select(p => p.ParameterType).ToArray());

                var methodIL = methodBuilder.GetILGenerator();

                var currentMethodIdLocal = methodIL.DeclareLocal(typeof(string));
                methodIL.Emit(OpCodes.Ldstr, map[method]);
                methodIL.Emit(OpCodes.Stloc, currentMethodIdLocal);

                var argumentsLocal = methodIL.DeclareLocal(typeof(object[]));
                methodIL.Emit(OpCodes.Ldc_I4, parameters.Length);
                methodIL.Emit(OpCodes.Newarr, typeof(object));
                for (var i = 0; i < parameters.Length; i++)
                {
                    methodIL.Emit(OpCodes.Dup);
                    methodIL.Emit(OpCodes.Ldc_I4, i);
                    methodIL.Emit(OpCodes.Ldarg, i + 1);

                    if (parameters[i].ParameterType.IsValueType)
                        methodIL.Emit(OpCodes.Box, parameters[i].ParameterType);

                    methodIL.Emit(OpCodes.Stelem_Ref);
                }
                methodIL.Emit(OpCodes.Stloc, argumentsLocal);

                var resultLocal = methodIL.DeclareLocal(typeof(object));
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, callbackFieldBuilder);
                methodIL.Emit(OpCodes.Ldloc, currentMethodIdLocal);
                methodIL.Emit(OpCodes.Ldloc, argumentsLocal);
                methodIL.Emit(OpCodes.Callvirt, CallBackInvokeMethodInfo);
                methodIL.Emit(OpCodes.Stloc, resultLocal);

                if (returnType != typeof(void))
                {
                    methodIL.Emit(OpCodes.Ldloc, resultLocal);
                    var instruction = returnType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass;
                    methodIL.Emit(instruction, returnType);
                }

                methodIL.Emit(OpCodes.Ret);

                methodMap.Add(method.Name, methodBuilder);
            }

            foreach (var property in interfaceType.GetProperties())
            {
                var propertyBuilder = typeBuilder.DefineProperty(property.Name,
                                                                 property.Attributes,
                                                                 property.PropertyType,
                                                                 property.GetIndexParameters()
                                                                         .Select(p => p.ParameterType)
                                                                         .ToArray());

                if (property.GetMethod != null)
                    propertyBuilder.SetGetMethod(methodMap["get_" + property.Name]);

                if (property.SetMethod != null)
                    propertyBuilder.SetSetMethod(methodMap["set_" + property.Name]);
            }

            foreach (var eventInfo in interfaceType.GetEvents())
            {
                var eventBuilder = typeBuilder.DefineEvent(eventInfo.Name,
                                                           eventInfo.Attributes,
                                                           eventInfo.EventHandlerType);

                if (eventInfo.AddMethod != null)
                    eventBuilder.SetAddOnMethod(methodMap["add_" + eventInfo.Name]);

                if (eventInfo.RemoveMethod != null)
                    eventBuilder.SetAddOnMethod(methodMap["remove_" + eventInfo.Name]);
            }

            return typeBuilder.CreateType();
        }

        #endregion

        #region Constructors

        static ProxyType()
        {
            CallBackType = typeof(Func<string, object[], object>);
            CallBackInvokeMethodInfo = CallBackType.GetMethod(nameof(Action.Invoke));

            DefaultObjectConstructorInfo = typeof(object).GetConstructor(Type.EmptyTypes);
            Cache = new Cache<Type, ProxyType>();
        }

        internal ProxyType(Type contractType)
        {
            ContractType = contractType;

            var methodMap = contractType.GetMethods().ToDictionary(m => m, m => Guid.NewGuid().ToString("N"));
            MethodsMap = methodMap.ToDictionary(p => p.Value, p => p.Key);

            ProducerType = GenerateMockType(contractType, methodMap);
        }

        #endregion

        #region Properties

        public Type ContractType { get; }

        public Type ProducerType { get; }

        internal IReadOnlyDictionary<string, MethodInfo> MethodsMap { get; }

        #endregion
    }
}