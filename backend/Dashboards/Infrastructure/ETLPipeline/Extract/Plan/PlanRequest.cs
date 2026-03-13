using Infrastructure.ETLPipeline.Extract.Validation;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Infrastructure.ETLPipeline.Extract.Plan
{
    public class PlanRequest(HttpClient httpClient): IPlanRequest
    {
        public async Task<List<PlanResponse>> GetAllPlansAsync(
            string token,
            Guid eduGroupId,
            int semester)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var request = new PlanRequestDto
            {
                EduGroupId = eduGroupId,
                Semester = semester
            };

            var response = await httpClient.PostAsJsonAsync(ApiRoutes.PlanUrl, request);

            RequestValidation.ValidateResponse(response);

            var plans = await response.Content.ReadFromJsonAsync<List<PlanResponse>>();

            return plans;
        }
    }
}
