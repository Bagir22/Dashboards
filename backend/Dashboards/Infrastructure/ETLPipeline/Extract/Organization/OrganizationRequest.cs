using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.Organization
{
    public class OrganizationRequest(HttpClient httpClient) : IOrganizationRequest
    {
        public async Task<List<OrganizationResponse>> GetAllOrganizationsAsync( string token )
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.OrganizationUrl );

            RequestValidation.ValidateResponse( response );

            var organizations = await response.Content.ReadFromJsonAsync<List<OrganizationResponse>>();

            return organizations!;
        }
    }
}
