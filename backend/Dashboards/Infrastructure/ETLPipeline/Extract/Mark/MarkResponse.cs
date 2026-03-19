using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Mark
{
    public class MarkResponse
    {
        [JsonPropertyName( "id" )]
        public required string MarkId { get; set; }
        [JsonPropertyName( "markName" )]
        public required string Name { get; set; }
        [JsonPropertyName( "markValue" )]
        public required int Value { get; set; }
        [JsonPropertyName( "isGoodMark" )]
        public required bool isGoodMark { get; set; }
    }
}
