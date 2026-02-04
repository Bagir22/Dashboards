using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.StudyForm
{
    public class StudyFormRequest( HttpClient httpClient ): IStudyFormRequest
    {
        public async Task<List<StudyFormResponse>> GetAllStudyFormsAsync( string token )
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.StudyFormUrl );

            RequestValidation.ValidateResponse( response );

            var forms = await response.Content.ReadFromJsonAsync<List<StudyFormResponse>>();

            return forms!;
        }
    }
}
