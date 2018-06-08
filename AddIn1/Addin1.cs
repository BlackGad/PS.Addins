using System;
using Contracts;

namespace AddIn1
{
    class Addin1 : ITestContract
    {
        #region ITestContract Members

        public int this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int Property { get; set; }
        public event EventHandler Event;

        public float Function(int first, string second)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}