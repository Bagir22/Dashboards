using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.AchivmentCategory
{
    public class AchivmentCategoryResponse
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}
