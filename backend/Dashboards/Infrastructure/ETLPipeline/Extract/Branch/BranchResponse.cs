using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Branch
{
    public class BranchResponse
    {
        [JsonPropertyName( "dictFilialExternalId" )]
        public required string BranchExternalId { get; set; }
        [JsonPropertyName( "filialName" )]
        public required string Name { get; set; }
    }
}
