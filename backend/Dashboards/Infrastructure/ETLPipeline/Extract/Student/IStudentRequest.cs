namespace Infrastructure.ETLPipeline.Extract.Student
{
    public interface IStudentRequest
    {
        public Task<StudentsResponse> GetAllStudentsByDateAsync( string token, DateTime dateTime, int pageNumber );
    }
}
