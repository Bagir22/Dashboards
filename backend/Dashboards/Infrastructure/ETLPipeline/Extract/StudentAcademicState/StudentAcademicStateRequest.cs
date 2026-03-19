using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.StudentAcademicState
{
    public class StudentAcademicStateRequest( HttpClient httpClient ): IStudentAcademicStateRequest
    {
        public async Task<List<StudentAcademicStateResponse>> GetAllStudentAcademicStatesAsync( string token )
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.StudentAcademicStateUrl );

            RequestValidation.ValidateResponse( response );

            var states = await response.Content.ReadFromJsonAsync<List<StudentAcademicStateResponse>>();

            return states!;
        }
    }
}
