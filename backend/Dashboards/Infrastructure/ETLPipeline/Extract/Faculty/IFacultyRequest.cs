namespace Infrastructure.ETLPipeline.Extract.Faculty
{
    public interface IFacultyRequest
    {
        public Task<List<FacultyResponse>> GetAllFacultiesAsync( string token );
    }
}
