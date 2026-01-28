namespace Infrastructure.ETLPipeline.Extract.EducationProgram
{
    public interface IEducationProgramRequest
    {
        public Task<List<EducationProgramResponse>> GetAllEducationProgramsAsync( string token );
    }
}
