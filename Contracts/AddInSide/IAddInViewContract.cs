using System;

namespace Contracts.AddInSide
{
    public interface IAddInViewContract
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

        #endregion
    }
}