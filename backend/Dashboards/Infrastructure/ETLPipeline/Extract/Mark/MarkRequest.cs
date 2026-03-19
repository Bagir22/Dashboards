using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.Mark
{
    public class MarkRequest( HttpClient httpClient ): IMark
    {
        public async Task<List<MarkResponse>> GetAllMarksAsync(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.MarksUrl );

            RequestValidation.ValidateResponse( response );

            var marksResponses = await response.Content.ReadFromJsonAsync<List<MarkResponse>>();

            return marksResponses;
        }
    }
}
