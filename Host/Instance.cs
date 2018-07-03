using System;
using Contracts;
using Contracts.HostSide;

namespace Host
{
    class Instance : IHostViewContract
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

        public event EventHandler Event
        {
            add { }
            remove { }
        }

        public float Function(int first, string second)
        {
            return default(float);
        }

        #endregion
    }
}