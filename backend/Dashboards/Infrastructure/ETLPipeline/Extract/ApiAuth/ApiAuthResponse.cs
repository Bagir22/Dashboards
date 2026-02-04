using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.ApiAuth
{
    public class ApiAuthResponse
    {
        [JsonPropertyName( "access_token" )]
        public required string AccessToken { get; set; }
    }
}
