namespace Infrastructure.ETLPipeline.Extract.Organization
{
    public interface IOrganizationRequest
    {
        public Task<List<OrganizationResponse>> GetAllOrganizationsAsync( string token );
    }
}
