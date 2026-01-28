namespace Infrastructure.ETLPipeline.Extract.Citizenship
{
    public interface ICitizenshipRequest
    {
        public Task<List<CitizenshipResponse>> GetAllCitizenshipsAsync( string token );
    }
}
