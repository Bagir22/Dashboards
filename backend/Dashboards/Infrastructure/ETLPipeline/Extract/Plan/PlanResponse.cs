using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Plan
{
    public class PlanResponse
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}
