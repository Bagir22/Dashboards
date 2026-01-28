using System.Net.Http.Headers;
using System.Net.Http.Json;
using Infrastructure.ETLPipeline.Extract.Validation;
using Microsoft.Extensions.Options;

namespace Infrastructure.ETLPipeline.Extract.ApiAuth
{
    public class AuthBodySettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public string GrantType { get; set; } = "client_credentials";
    }

    public class ApiAuthRequest( HttpClient httpClient, IOptions<AuthBodySettings> options ): IApiAuthRequest
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly AuthBodySettings _settings = options.Value;


        public async Task<string> GetTokenAsync()
        {
            var formData = new Dictionary<string, string>
            {
                { "client_id", _settings.ClientId },
                { "client_secret", _settings.ClientSecret},
                { "scope", _settings.Scope },
                { "grant_type", _settings.GrantType }
            };

            var content = new FormUrlEncodedContent( formData );
            content.Headers.ContentType = new MediaTypeHeaderValue( "application/x-www-form-urlencoded" );


            var response = await _httpClient.PostAsync( ApiRoutes.ApiAuthUrl, content );

            RequestValidation.ValidateResponse( response );

            var deserialized = await response.Content.ReadFromJsonAsync<ApiAuthResponse>();

            return deserialized!.AccessToken;
        }
    }
}
