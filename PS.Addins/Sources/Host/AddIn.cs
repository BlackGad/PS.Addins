using System;

namespace PS.Addins.Host
{
    public class AddIn
    {
        #region Constructors

        public AddIn(Version version,
                     string assemblyPath,
                     string addinTypeName,
                     Type[] supportedContracts)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (assemblyPath == null) throw new ArgumentNullException(nameof(assemblyPath));
            if (addinTypeName == null) throw new ArgumentNullException(nameof(addinTypeName));

            AssemblyPath = assemblyPath;
            AddinTypeName = addinTypeName;
            SupportedContracts = supportedContracts;
            Version = version;
        }

        #endregion

        #region Properties

        public string AddinTypeName { get; }

        public string AssemblyPath { get; }

        public Type[] SupportedContracts { get; }

        public Version Version { get; }

        #endregion
    }
}