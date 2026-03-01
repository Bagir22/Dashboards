namespace Infrastructure.ETLPipeline.Extract.Mark
{
    public interface IMark
    {
        public Task<List<MarkResponse>> GetAllMarksAsync( string token );
    }
}
