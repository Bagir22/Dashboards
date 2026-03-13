using Infrastructure.ETLPipeline.Extract.Validation;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Infrastructure.ETLPipeline.Extract.SheetDiscipline
{
    public class SheetDisciplineRequest(HttpClient httpClient): ISheetDisciplineRequest
    {
        public async Task<List<SheetDisciplineResponse>> GetAllSheetsDisciplineAsync(
            string token,
            Guid eduGroupId,
            Guid disciplineId,
            DateTime markDate,
            int semester,
            bool isCurrentSemester)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var request = new SheetDisciplineRequestDto
            {
                EduGroupId = eduGroupId,
                DisciplineId = disciplineId,
                MarkDate = markDate,
                Semester = semester,
                IsCurrentSemester = isCurrentSemester
            };

            var response = await httpClient.PostAsJsonAsync(ApiRoutes.SheetDisciplineUrl, request);

            RequestValidation.ValidateResponse(response);

            var sheetResponses = await response.Content.ReadFromJsonAsync<List<SheetDisciplineResponse>>();

            return sheetResponses;
        }
    }
}
