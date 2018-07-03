using System;
using Contracts.AddInSide;

namespace AddIn1
{
    class Addin : IAddInViewContract
    {
        #region Properties

        public int this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int Property { get; set; }

        #endregion

        #region Events

        public event EventHandler Event;

        #endregion

        #region IAddInViewContract Members

        public float Function(int first, string second)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}