using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Order
{
    public class OrderResponse
    {
        [JsonPropertyName("ordersExternalId")]
        public required string Id { get; set; }
        [JsonPropertyName("dictOrdersCategories")]
        public required DictOrdersCategories DictOrdersCategories { get; set; }
        [JsonPropertyName("orderDate")]
        public required DateTime Date { get; set; }
    }

    public class DictOrdersCategories
    {
        [JsonPropertyName("dictOrderType")]
        public DictOrderType? DictOrderType { get; set; }
        [JsonPropertyName("categoryName")]
        public required string Category { get; set; }
    }

    public class DictOrderType
    {
        [JsonPropertyName("enums")]
        public required List<int> Enums { get; set; }
    }
}
