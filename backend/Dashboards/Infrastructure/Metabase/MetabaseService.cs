using Application.Contracts;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Threading;

namespace Infrastructure.Metabase;

public class MetabaseService: IMetabaseService
{
    private readonly HttpClient _httpClient;
    private readonly MetabaseSettings _settings;
    private readonly SemaphoreSlim _sessionLock = new(1, 1);

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
    //Реализация интерфейса
    public async Task<string> GetCardDataJsonAsync(int cardId)
    {
        return await GetCardDataJsonAsync(cardId, CancellationToken.None);
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
            return await ExecuteCardQueryAsync(cardId, cancellationToken);
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
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/card/{cardId}/query/json");
        request.Headers.Add("X-Metabase-Session", _sessionId);

        request.Content = JsonContent.Create(new object());

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private class MetabaseSessionResponse
    {
        public string? Id { get; set; }
    }
}

// Настройки
public class MetabaseSettings
{
    public string BaseUrl => Environment.GetEnvironmentVariable("METABASE_URL") ?? "http://localhost:3000/";

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
