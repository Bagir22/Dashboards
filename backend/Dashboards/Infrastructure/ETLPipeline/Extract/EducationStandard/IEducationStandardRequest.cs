namespace Infrastructure.ETLPipeline.Extract.EducationStandard
{
    public interface IEducationStandardRequest
    {
        public Task<List<EducationStandardResponse>> GetAllEducationStandardsAsync( string token );
    }
}
