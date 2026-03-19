using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.EducationProgram
{
    public class EducationProgramRequest(HttpClient httpClient) : IEducationProgramRequest
    {
        public async Task<List<EducationProgramResponse>> GetAllEducationProgramsAsync( string token )
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.EducationProgramUrl );

            RequestValidation.ValidateResponse( response );

            var educationPrograms = await response.Content.ReadFromJsonAsync<List<EducationProgramResponse>>();

            return educationPrograms!;
        }
    }
}
