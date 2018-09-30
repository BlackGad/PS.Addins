using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Contracts;
using Contracts.AddInSide;

namespace AddIn1
{
    class Addin1 : IAddInViewContract
    {
        private readonly List<EventHandler> _events;
        private IHostService _service;

        #region Constructors

        public Addin1()
        {
            _events = new List<EventHandler>();
        }

        #endregion

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

        public event EventHandler Event
        {
            add { _events.Add(value); }
            remove { _events.Remove(value); }
        }

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

            foreach (var @event in _events)
            {
                @event?.Invoke(this, EventArgs.Empty);
            }
        }

        public int RaiseServiceCall()
        {
            Log();
            return _service.ServiceCall("Hi!");
        }

        public void SetService(IHostService service)
        {
            Log();
            _service = service;
        }

        public ISpawnedObject SpawnObject()
        {
            Log();
            return new SpawnedObject();
        }

        #endregion

        #region Members

        private void Log([CallerMemberName] string methodName = null)
        {
            Console.WriteLine("*** Received: " + methodName + ", Domain: " + AppDomain.CurrentDomain.FriendlyName);
        }

        #endregion
    }

    class SpawnedObject : ISpawnedObject
    {
        #region ISpawnedObject Members

        public void Test()
        {
            Log();
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