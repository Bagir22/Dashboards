namespace Infrastructure.ETLPipeline
{
    public interface IDataSynchronizer
    {
        public Task InitialCreate();
        public Task UpdateData();
    }
}
