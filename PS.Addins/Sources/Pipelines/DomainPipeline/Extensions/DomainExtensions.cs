using System;
using System.Reflection;

namespace PS.Addins.Pipelines.Extensions
{
    public static class DomainExtensions
    {
        #region Static members

        public static T CreateObject<T>(this AppDomain domain, params object[] args)
        {
            var objectType = typeof(T);
            if (objectType.FullName == null) throw new InvalidOperationException();

            return (T)domain.CreateInstanceFromAndUnwrap(objectType.Assembly.Location,
                                                         objectType.FullName,
                                                         false,
                                                         BindingFlags.Default,
                                                         null,
                                                         args,
                                                         null,
                                                         null);
        }

        

        internal static DomainInvoker GetRemoteInvoker(this AppDomain domain, string pipelineId)
        {
            return domain.GetData(pipelineId + nameof(DomainInvoker)) as DomainInvoker;
        }

        internal static DomainVault GetVault(this AppDomain domain, string pipelineId)
        {
            return domain.GetData(pipelineId + nameof(DomainVault)) as DomainVault;
        }

        internal static void SetRemoteInvoker(this AppDomain domain, string pipelineId, DomainInvoker invoker)
        {
            domain.SetData(pipelineId + nameof(DomainInvoker), invoker);
        }

        internal static void SetVault(this AppDomain domain, string pipelineId, DomainVault vault)
        {
            domain.SetData(pipelineId + nameof(DomainVault), vault);
        }

        #endregion
    }
}