using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.EducationProgram
{
    public class EducationProgramResponse
    {
        [JsonPropertyName( "externalId" )]
        public required string EducationProgramId { get; set; }
        [JsonPropertyName( "name" )]
        public required string Name { get; set; }
    }
}
