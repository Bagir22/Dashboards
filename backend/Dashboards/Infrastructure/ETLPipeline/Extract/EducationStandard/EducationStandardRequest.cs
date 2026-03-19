using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.EducationStandard
{
    public class EducationStandardRequest(HttpClient httpClient) : IEducationStandardRequest
    {
        public async Task<List<EducationStandardResponse>> GetAllEducationStandardsAsync( string token )
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.EducationStandardUrl );

            RequestValidation.ValidateResponse( response );

            var educationStndards = await response.Content.ReadFromJsonAsync<List<EducationStandardResponse>>();

            return educationStndards!;
        }
    }
}
