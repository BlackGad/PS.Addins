using System;

namespace PS.Addins.Pipelines
{
    class ObjectIdentifier : MarshalByRefObject
    {
        #region Constructors

        public ObjectIdentifier(string pipelineId)
        {
            PipelineId = pipelineId ?? throw new ArgumentNullException(nameof(pipelineId));
        }

        #endregion

        #region Properties

        public string PipelineId { get; }

        #endregion
    }
}