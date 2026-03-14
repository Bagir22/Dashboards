namespace Infrastructure.ETLPipeline.Extract.SheetDiscipline
{
    public interface ISheetDisciplineRequest
    {
        public Task<List<SheetDisciplineResponse>> GetAllSheetsDisciplineAsync(string token, Guid eduGroupId, Guid disciplineId, DateTime markDate, int semester, bool isCurrentSemester);
    }
}
