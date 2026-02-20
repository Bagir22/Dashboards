namespace Infrastructure.ETLPipeline.Extract.Branch
{
    public interface IBranchRequest
    {
        public Task<List<BranchResponse>> GetAllBranchesAsync( string token );
    }
}
