using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Organization
{
    public class OrganizationResponse
    {
        [JsonPropertyName( "dictOrganizationExternalId" )]
        public required string DictOrganizationExternalId { get; set; }
        [JsonPropertyName( "organizationName" )]
        public required string OrganizationName { get; set; }
    }
}
