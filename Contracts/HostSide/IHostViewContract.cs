using System;

namespace Contracts.HostSide
{
    public interface IHostViewContract
    {
        #region Properties

        int this[int index] { get; set; }

        int Property { get; set; }

        #endregion

        #region Events

        event EventHandler Event;

        #endregion

        #region Members

        float Function(int first, string second);

        void RaiseEvent();

        int RaiseServiceCall();

        void SetService(IHostService service);

        ISpawnedObject SpawnObject();

        #endregion
    }
}