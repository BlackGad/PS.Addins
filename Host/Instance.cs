using System;
using System.Runtime.CompilerServices;
using Contracts.AddInSide;

namespace Host
{
    class Instance : IAddInViewContract
    {
        #region Properties

        public int this[int index]
        {
            get
            {
                Log();
                return default(int);
            }
            set { Log(); }
        }

        public int Property
        {
            get
            {
                Log();
                return default(int);
            }
            set { Log(); }
        }

        #endregion

        #region Events

        public event EventHandler Event;

        #endregion

        #region IAddInViewContract Members

        public float Function(int first, string second)
        {
            Log();
            return default(float);
        }

        public void RaiseEvent()
        {
            Log();
            Event?.Invoke(null, EventArgs.Empty);
        }

        #endregion

        #region Members

        private void Log([CallerMemberName] string methodName = null)
        {
            Console.WriteLine("*** Received: " + methodName + ", Domain: " + AppDomain.CurrentDomain.FriendlyName);
        }

        #endregion
    }
}