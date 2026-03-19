using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.Citizenship
{
    public class CitizenshipRequest( HttpClient httpClient ): ICitizenshipRequest
    {
        public async Task<List<CitizenshipResponse>> GetAllCitizenshipsAsync( string token )
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.CitizenshipUrl );

            RequestValidation.ValidateResponse( response );

            var citizenships = await response.Content.ReadFromJsonAsync<List<CitizenshipResponse>>();

            return citizenships!;
        }
    }
}
