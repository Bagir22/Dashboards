using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Discipline
{
    public class DisciplineResponse
    {
        [JsonPropertyName( "id" )]
        public required string DisciplineId { get; set; }
        [JsonPropertyName( "name" )]
        public required string Name { get; set; }
    }
}
