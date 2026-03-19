namespace Infrastructure.ETLPipeline.Synchronize
{
    public interface IDataSynchronizer
    {
        public Task InitialCreate();
        public Task UpdateData();
    }
}
