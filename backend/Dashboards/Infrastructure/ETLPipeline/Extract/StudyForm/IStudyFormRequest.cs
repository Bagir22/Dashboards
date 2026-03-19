namespace Infrastructure.ETLPipeline.Extract.StudyForm
{
    public interface IStudyFormRequest
    {
        public Task<List<StudyFormResponse>> GetAllStudyFormsAsync( string token );
    }
}
