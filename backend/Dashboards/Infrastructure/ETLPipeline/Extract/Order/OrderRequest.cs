using Infrastructure.ETLPipeline.Extract.Validation;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Infrastructure.ETLPipeline.Extract.Order
{
    public class OrderRequest(HttpClient httpClient) : IOrderRequest
    {
        public async Task<List<OrderResponse>> GetAllOrdersAsync(string token, string studentId)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var url = $"{ApiRoutes.OrderUrl}/{studentId}";

            var response = await httpClient.GetAsync(url);

            RequestValidation.ValidateResponse(response);

            var benefits = await response.Content.ReadFromJsonAsync<List<OrderResponse>>();

            return benefits!;
        }
    }
}
