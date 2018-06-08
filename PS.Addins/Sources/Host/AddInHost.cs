using System;

namespace PS.Addins.Host
{
    public class AddInHost
        {
            #region Members

            public AddIn[] FindAddIns()
            {
                return new[] { new AddIn(new Version(1, 0)) };
            }

            #endregion
        }
    
}
