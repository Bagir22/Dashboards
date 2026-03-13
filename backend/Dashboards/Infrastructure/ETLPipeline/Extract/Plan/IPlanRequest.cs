namespace Infrastructure.ETLPipeline.Extract.Plan
{
    public interface IPlanRequest
    {
        public Task<List<PlanResponse>> GetAllPlansAsync(string token, Guid eduGroupId, int semester);
    }
}
