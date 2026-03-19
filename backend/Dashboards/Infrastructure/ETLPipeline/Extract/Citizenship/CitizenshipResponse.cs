using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Citizenship
{
    public class CitizenshipResponse
    {
        [JsonPropertyName( "citizenshipExternalId" )]
        public required string CitizenshipExternalId { get; set; }
        [JsonPropertyName( "country" )]
        public required string Country { get; set; }
    }
}
