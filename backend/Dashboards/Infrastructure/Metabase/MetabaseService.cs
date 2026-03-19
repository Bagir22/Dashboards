using Application.Contracts;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Metabase;

public class MetabaseService: IMetabaseService
{
    private readonly HttpClient _httpClient;
    private readonly MetabaseSettings _settings;
    private string? _sessionId;
    private DateTime _sessionExpiresAt = DateTime.MinValue;

    public MetabaseService(HttpClient httpClient, IOptions<MetabaseSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        if (httpClient.BaseAddress == null)
        {
            httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

    }

    public async Task AuthenticateAsync(string email, string password)
    {
        var loginData = new { username = email, password = password };
        var response = await _httpClient.PostAsJsonAsync("api/session", loginData);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<MetabaseSessionResponse>();
        _sessionId = result?.Id;
        _sessionExpiresAt = DateTime.UtcNow.AddHours(12);
    }

    public async Task<string> GetCardDataJsonAsync(int cardId, CancellationToken cancellationToken)
    {
        // Проверка сессии
        if (string.IsNullOrEmpty(_sessionId) || DateTime.UtcNow >= _sessionExpiresAt)
        {
            await AuthenticateAsync(_settings.Email, _settings.Password);
        }

        try
        {
            var cardUuid = await ExecuteCardQueryAsync(cardId, cancellationToken);
            return await GetCardDataByUuid(cardUuid, cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _sessionId = null;
            await AuthenticateAsync(_settings.Email, _settings.Password);
            return await ExecuteCardQueryAsync(cardId, cancellationToken);
        }
    }

    private async Task<string> ExecuteCardQueryAsync(int cardId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/card/{cardId}");
        request.Headers.Add("X-Metabase-Session", _sessionId);

        request.Content = JsonContent.Create(new object());

        var response = await _httpClient.SendAsync(request, cancellationToken);

        var cardResponse = await response.Content.ReadFromJsonAsync<CardResponse>(cancellationToken);
        if (null == cardResponse)
        {
            throw new HttpRequestException();
        }

        return cardResponse.Id;   
    }

    public async Task<string> GetCardDataByUuid(string cardId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/public/card/{cardId}/query");
        request.Headers.Add("X-Metabase-Session", _sessionId);

        request.Content = JsonContent.Create(new object());

        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private class MetabaseSessionResponse
    {
        public string? Id { get; set; }
    }

    private class CardResponse
    {
        [JsonPropertyName("public_uuid")]
        public string Id { get; set; } = null!;
    }
}

public class MetabaseSettings
{
    public string BaseUrl { get; set; } = "http://metabase:3000/";
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
