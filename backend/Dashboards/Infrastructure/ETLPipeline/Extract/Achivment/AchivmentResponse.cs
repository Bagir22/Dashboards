using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Achivment
{
    public class AchivmentResponse
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("status")]
        public required Status Status { get; set; }
        [JsonPropertyName("period")]
        public required Period Period { get; set; }
    }

    public class Status
    {
        [JsonPropertyName("status")]
        public required StatusIn StatusIn { get; set; }
    }

    public class StatusIn
    {
        [JsonPropertyName("value")]
        public required int Value { get; set; }
    }

    public class Period
    {
        [JsonPropertyName("begin")]
        public required DateTime Begin { get; set; }
    }
}
