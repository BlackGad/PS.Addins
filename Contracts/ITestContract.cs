using System;

namespace Contracts
{
    public interface ITestContract
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

        #endregion
    }
}