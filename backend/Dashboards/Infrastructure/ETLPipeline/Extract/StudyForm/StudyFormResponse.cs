using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.StudyForm
{
    public class StudyFormResponse
    {
        [JsonPropertyName( "DictStudyFormExternalId" )]
        public required string DictStudyFormExternalId { get; set; }
        [JsonPropertyName( "studyFormName" )]
        public required string StudyFormName { get; set; }
    }
}
