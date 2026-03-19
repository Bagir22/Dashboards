using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.OrderCategory
{
    public class OrderCategoryResponse
    {
        [JsonPropertyName("dictOrdersCategoryExternalId")]
        public required string Id { get; set; }
        [JsonPropertyName("categoryName")]
        public required string Name { get; set; }
    }
}
