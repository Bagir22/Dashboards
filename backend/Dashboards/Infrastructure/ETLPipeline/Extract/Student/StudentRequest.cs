using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Infrastructure.ETLPipeline.Extract.Utils;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.Student
{
    public class StudentRequest( HttpClient httpClient, FileReader fileReader ): IStudentRequest
    {
        public async Task<StudentsResponse> GetAllStudentsByDateAsync( string token, DateTime dateTime, int pageNumber )
        {
            string jsonBody = await fileReader.ReadRequestBodyFromFileAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var requestBody = JsonSerializer.Deserialize<GetStudentsRequestBody>( jsonBody, options );

            requestBody!.filter.contingentDate = dateTime.ToString( "o" );

            requestBody!.page.pageNumber = pageNumber;

            jsonBody = JsonSerializer.Serialize( requestBody );

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var content = new StringContent( jsonBody, Encoding.UTF8, "application/json" );

            var response = await httpClient.PostAsync( ApiRoutes.ContingentUrl, content );

            RequestValidation.ValidateResponse( response );

            var students = await response.Content.ReadFromJsonAsync<StudentsResponse>();

            return students!;
        }
    }
}
