using System;
using Contracts.AddInSide;
using Contracts.HostSide;

namespace Host
{
    class Instance : IAddInViewContract
    {
        #region ITestContract Members

        public int this[int index]
        {
            get { return default(int); }
            set { }
        }

        public int Property
        {
            get { return default(int); }
            set { }
        }

        public event EventHandler Event;

        public float Function(int first, string second)
        {
            Event?.Invoke(this, EventArgs.Empty);
            return default(float);
        }

        #endregion
    }
}