namespace Infrastructure.ETLPipeline.Extract.TrainingLevel
{
    public interface ITrainingLevel
    {
        public Task<List<TrainingLevelResponse>> GetAllTrainingLevelsAsync( string token );
    }
}
