using System;
using System.Linq;
using System.Reflection;
using PS.Addins.Extensions;
using PS.Addins.Pipelines.Base;

namespace PS.Addins.Pipelines
{
    public class DirectPipeline : IPipeline
    {
        #region IPipeline Members

        public T CreateObject<T>(string assemblyLocation, string typeName)
        {
            var assembly = Assembly.LoadFrom(assemblyLocation);
            var type = assembly.GetAssemblyTypes()
                               .FirstOrDefault(t => string.Equals(t.FullName,
                                                                  typeName,
                                                                  StringComparison.InvariantCultureIgnoreCase));
            if (type == null) throw new TypeLoadException($"Could not find '{typeName}'");
            var instance = Activator.CreateInstance(type);
            if (instance is T variable) return variable;

            return ProxyType.Create<T>((method, args) => type.GetSimilarMethod(method).Invoke(instance, args));
        }

        public void Dispose()
        {
        }

        #endregion
    }
}