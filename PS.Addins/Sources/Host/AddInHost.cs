using PS.Addins.Adapters.Base;

namespace PS.Addins.Host
{
    public class AddInHost
    {
        #region Members

        public AddInInstance Create(AddIn addIn, AddInSidesAdapter addInSidesAdapter)
        {
            return new AddInInstance(addIn, addInSidesAdapter.Instantiate(addIn));
        }

        #endregion
    }
}