using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Benifit
{
    public class BenefitResponse
    {
        [JsonPropertyName( "dictBenefitExternalId" )]
        public required string DictBenefitExternalId { get; set; }
        [JsonPropertyName( "benefitName" )]
        public required string BenefitName { get; set; }
    }
}
