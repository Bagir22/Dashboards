using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.TrainingLevel
{
    public class TrainingLevelResponse
    {
        [JsonPropertyName( "externalId" )]
        public required string TrainingLevelExternalId { get; set; }
        [JsonPropertyName( "trainingLevelName" )]
        public required string Name { get; set; }
    }
}
