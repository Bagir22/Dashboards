using Infrastructure.ETLPipeline.Extract.Validation;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Infrastructure.ETLPipeline.Extract.OrderCategory
{
    public class OrderCategoryRequest(HttpClient httpClient) : IOrderCategoryRequest
    {
        public async Task<List<OrderCategoryResponse>> GetAllOrderCategoriesAsync(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync(ApiRoutes.OrderCategoryUrl);

            RequestValidation.ValidateResponse(response);

            var benefits = await response.Content.ReadFromJsonAsync<List<OrderCategoryResponse>>();

            return benefits!;
        }
    }
}
