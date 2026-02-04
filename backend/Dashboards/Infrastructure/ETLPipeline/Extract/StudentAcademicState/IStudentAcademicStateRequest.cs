namespace Infrastructure.ETLPipeline.Extract.StudentAcademicState
{
    public interface IStudentAcademicStateRequest
    {
        public Task<List<StudentAcademicStateResponse>> GetAllStudentAcademicStatesAsync( string token );
    }
}
