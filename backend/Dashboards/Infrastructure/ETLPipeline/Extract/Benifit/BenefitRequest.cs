using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.Benifit
{
    public class BenefitRequest( HttpClient httpClient ) : IBenefitRequest
    {
        public async Task<List<BenefitResponse>> GetAllBenefitsAsync( string token )
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.BenefitUrl );

            RequestValidation.ValidateResponse( response );

            var benefits = await response.Content.ReadFromJsonAsync<List<BenefitResponse>>();

            return benefits!;
        }
    }
}
