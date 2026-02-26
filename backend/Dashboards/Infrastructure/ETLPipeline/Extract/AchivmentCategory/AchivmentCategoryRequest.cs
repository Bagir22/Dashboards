using Infrastructure.ETLPipeline.Extract.Validation;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Infrastructure.ETLPipeline.Extract.AchivmentCategory
{
    public class AchivmentCategoryRequest(HttpClient httpClient): IAchivmentCategoryRequest
    {
        public async Task<List<AchivmentCategoryResponse>> GetAllAchivmentCategoriesAsync(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync(ApiRoutes.AchivmentCategoryUrl);

            RequestValidation.ValidateResponse(response);

            var benefits = await response.Content.ReadFromJsonAsync<List<AchivmentCategoryResponse>>();

            return benefits!;
        }
    }
}
