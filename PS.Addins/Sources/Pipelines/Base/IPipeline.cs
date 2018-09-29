namespace PS.Addins.Pipelines.Base
{
    public interface IPipeline
    {
        #region Members

        void Dispose();
        T Facade<T>();

        #endregion
    }
}