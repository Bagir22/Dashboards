using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.EducationStandard
{
    public class EducationStandardResponse
    {
        [JsonPropertyName( "educationStandardExternalId" )]
        public required string EducationStandardExternalId { get; set; }
        [JsonPropertyName( "name" )]
        public required string Name { get; set; }
    }
}
