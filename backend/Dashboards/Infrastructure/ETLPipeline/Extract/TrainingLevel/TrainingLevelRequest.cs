using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;

namespace Infrastructure.ETLPipeline.Extract.TrainingLevel
{
    public class TrainingLevelRequest( HttpClient httpClient ): ITrainingLevel
    {
        public async Task<List<TrainingLevelResponse>> GetAllTrainingLevelsAsync(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue( "Bearer", token );

            var response = await httpClient.GetAsync( ApiRoutes.TrainingLevelUrl );

            RequestValidation.ValidateResponse( response );

            var trainingLevels = await response.Content.ReadFromJsonAsync<List<TrainingLevelResponse>>();

            return trainingLevels!;
        }
    }
}
