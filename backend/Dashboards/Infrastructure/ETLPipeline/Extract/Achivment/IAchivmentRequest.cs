namespace Infrastructure.ETLPipeline.Extract.Achivment
{
    public interface IAchivmentRequest
    {
        public Task<List<AchivmentResponse>> GetAllAchivmentsAsync(string token, string studentId, string categoryId);
    }
}
