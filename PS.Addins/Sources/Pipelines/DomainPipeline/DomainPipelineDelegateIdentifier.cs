using System;

namespace PS.Addins.Pipelines
{
    class DomainPipelineDelegateIdentifier : MarshalByRefObject
    {
        #region Constructors

        public DomainPipelineDelegateIdentifier(Type delegateType)
        {
            DelegateType = delegateType ?? throw new ArgumentNullException(nameof(delegateType));
        }

        #endregion

        #region Properties

        public Type DelegateType { get; }

        #endregion
    }
}