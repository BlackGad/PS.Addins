using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PS.Addins.Host
{
    class AddInHostView
    {
        #region Constants

        private static readonly MethodInfo AddInHostViewAggregationCallMethodInfo;
        private static readonly Type AddInHostViewAggregationType;
        private static readonly ConstructorInfo DefaultObjectConstructorInfo;

        #endregion

        #region Static members

        private static Type GenerateHostSideAdapterType(Type contractInterfaceType, Dictionary<MethodInfo, string> map)
        {
            var assemblyName = new AssemblyName($"{contractInterfaceType.Name}_{Guid.NewGuid().ToString("N")}");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("module");

            var proxyTypeName = $"{contractInterfaceType.Name}_{Guid.NewGuid().ToString("N")}";
            var typeBuilder = moduleBuilder.DefineType(proxyTypeName);
            typeBuilder.AddInterfaceImplementation(contractInterfaceType);

            var proxyFieldBuilder = typeBuilder.DefineField("_proxy", AddInHostViewAggregationType, FieldAttributes.Private);

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
                                                                   CallingConventions.Standard,
                                                                   new[] { AddInHostViewAggregationType });

            var constructorIL = constructorBuilder.GetILGenerator();
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Call, DefaultObjectConstructorInfo);
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Ldarg_1);
            constructorIL.Emit(OpCodes.Stfld, proxyFieldBuilder);
            constructorIL.Emit(OpCodes.Ret);

            var methodMap = new Dictionary<string, MethodBuilder>();
            var contractMethods = contractInterfaceType.GetMethods();
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

                var resultLocal = methodIL.DeclareLocal(typeof(Type));
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, proxyFieldBuilder);
                methodIL.Emit(OpCodes.Ldloc, currentMethodIdLocal);
                methodIL.Emit(OpCodes.Ldloc, argumentsLocal);
                methodIL.Emit(OpCodes.Callvirt, AddInHostViewAggregationCallMethodInfo);
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

            foreach (var property in contractInterfaceType.GetProperties())
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

            foreach (var eventInfo in contractInterfaceType.GetEvents())
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

        static AddInHostView()
        {
            AddInHostViewAggregationType = typeof(AddInHostViewAggregation);

            DefaultObjectConstructorInfo = typeof(object).GetConstructor(Type.EmptyTypes);
            AddInHostViewAggregationCallMethodInfo = AddInHostViewAggregationType.GetMethod(nameof(AddInHostViewAggregation.CallMethod));
        }

        public AddInHostView(Type contractType)
        {
            ContractType = contractType;

            var methodMap = contractType.GetMethods().ToDictionary(m => m, m => Guid.NewGuid().ToString("N"));
            ContractMethodsMap = methodMap.ToDictionary(p => p.Value, p => p.Key);

            HostSideAdapterType = GenerateHostSideAdapterType(contractType, methodMap);
        }

        #endregion

        #region Properties

        public IReadOnlyDictionary<string, MethodInfo> ContractMethodsMap { get; }

        public Type ContractType { get; }
        public Type HostSideAdapterType { get; }

        #endregion
    }
}