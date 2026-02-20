using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Faculty;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.Branch
{
    public class BranchRequest( HttpClient httpClient ): IBranchRequest
    {
        public async Task<List<BranchResponse>> GetAllBranchesAsync(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.BranchUrl );

            RequestValidation.ValidateResponse( response );

            var branches = await response.Content.ReadFromJsonAsync<List<BranchResponse>>();

            return branches!;
        }
    }
}
