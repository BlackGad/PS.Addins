using PS.Addins.Adapters.Base;

namespace PS.Addins.Host
{
    public class AddInHost
    {
        #region Members

        public AddInInstance Create(AddIn addIn, AddInPipeline addInPipeline)
        {
            return new AddInInstance(addIn, addInPipeline.Instantiate(addIn));
        }

        #endregion
    }
}