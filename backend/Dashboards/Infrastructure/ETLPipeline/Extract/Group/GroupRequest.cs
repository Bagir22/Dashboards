using Infrastructure.ETLPipeline.Extract.Validation;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Infrastructure.ETLPipeline.Extract.Group
{
    public class GroupRequest(HttpClient httpClient): IGroupRequest
    {
        public async Task<List<GroupResponse>> GetAllGroupsAsync(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync(ApiRoutes.GroupUrl);

            RequestValidation.ValidateResponse(response);

            var groupsResponses = await response.Content.ReadFromJsonAsync<List<GroupResponse>>();

            return groupsResponses;
        }
    }
}
