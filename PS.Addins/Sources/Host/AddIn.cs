using System;

namespace PS.Addins.Host
{
    public class AddIn
    {
        #region Constructors

        public AddIn(Version version)
        {
            Version = version;
        }

        #endregion

        #region Properties

        public Version Version { get; }

        #endregion

        #region Members

        public AddInInstance Create()
        {
            return new AddInInstance();
        }

        #endregion
    }
}