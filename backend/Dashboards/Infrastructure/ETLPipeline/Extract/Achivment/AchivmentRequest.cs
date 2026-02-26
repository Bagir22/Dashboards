using Infrastructure.ETLPipeline.Extract.Validation;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Infrastructure.ETLPipeline.Extract.Achivment
{
    public class AchivmentRequest(HttpClient httpClient): IAchivmentRequest
    {
        public async Task<List<AchivmentResponse>> GetAllAchivmentsAsync(string token, string studentId, string categoryId)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var url = $"{ApiRoutes.AchivmentsUrl}/{studentId}/{categoryId}";

            var response = await httpClient.GetAsync(url);

            RequestValidation.ValidateResponse(response);

            var benefits = await response.Content.ReadFromJsonAsync<List<AchivmentResponse>>();

            return benefits!;
        }
    }
}
