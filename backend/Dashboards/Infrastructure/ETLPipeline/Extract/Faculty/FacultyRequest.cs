using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.Faculty
{
    public class FacultyRequest( HttpClient httpClient ): IFacultyRequest
    {
        public async Task<List<FacultyResponse>> GetAllFacultiesAsync( string token )
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.FacultyUrl );

            RequestValidation.ValidateResponse( response );

            var faculties = await response.Content.ReadFromJsonAsync<List<FacultyResponse>>();

            return faculties!;
        }
    }
}
