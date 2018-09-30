using System;
using System.Runtime.CompilerServices;
using Contracts;

namespace Host
{
    //[Serializable]
    class HostService : //MarshalByRefObject,
                        IHostService
    {
        #region IHostService Members

        public int ServiceCall(string b)
        {
            Log();
            return 42;
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