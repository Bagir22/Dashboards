namespace Infrastructure.ETLPipeline.Extract.Discipline
{
    public interface IDiscipline
    {
        public Task<List<DisciplineResponse>> GetAllDisciplinesAsync( string token );
    }
}
