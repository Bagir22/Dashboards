using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.Discipline
{
    public class DisciplineRequest( HttpClient httpClient ): IDiscipline
    {
        public async Task<List<DisciplineResponse>> GetAllDisciplinesAsync(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.DisciplinesUrl );

            RequestValidation.ValidateResponse( response );

            var disciplinesResponses = await response.Content.ReadFromJsonAsync<List<DisciplineResponse>>();

            return disciplinesResponses!;
        }
    }
}
