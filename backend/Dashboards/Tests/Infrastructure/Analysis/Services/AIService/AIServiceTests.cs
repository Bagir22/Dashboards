using System.Net;
using Application.Contracts;
using FluentAssertions;
using Infrastructure.Analysis.Services.AIService;
using Moq;
using Moq.Protected;
using System.Text;
using System.Text.Json;
using Xunit;
using AIServiceAlias = Infrastructure.Analysis.Services.AIService.AIService;

namespace Tests.Infrastructure.Analysis.Services.AIService;

public class AIServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly AIServiceAlias _aiService;

    public AIServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _aiService = new AIServiceAlias(_httpClient);
    }

    #region Успешные сценарии

    [Fact]
    public async Task GetCompletionAsync_ShouldReturnStream_WhenApiRespondsSuccessfully()
    {
        var cardData = "test card data";
        var expectedContent = new[] { "Анализ:", "данных", "завершен" };
        var mockResponse = CreateMockSSEResponse(expectedContent);

        SetupHttpResponse(mockResponse);

        var result = _aiService.GetCompletionAsync(cardData, CancellationToken.None);
        var actualContent = new List<string>();

        await foreach (var chunk in result)
        {
            actualContent.Add(chunk);
        }

        actualContent.Should().HaveCount(3);
        actualContent.Should().Equal(expectedContent);
    }

    [Fact]
    public async Task GetCompletionAsync_ShouldSendCorrectRequest()
    {
        var cardData = "test data for analysis";
        HttpRequestMessage capturedRequest = null;

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("data: [DONE]\n\n", Encoding.UTF8, "text/event-stream")
            });

        var result = _aiService.GetCompletionAsync(cardData, CancellationToken.None);
        await foreach (var _ in result) { }

        capturedRequest.Should().NotBeNull();
        capturedRequest.Method.Should().Be(HttpMethod.Post);
        capturedRequest.RequestUri.ToString().Should().Contain("openrouter.ai");

        var requestContent = await capturedRequest.Content.ReadAsStringAsync();
        var requestJson = JsonSerializer.Deserialize<JsonElement>(requestContent);

        requestJson.GetProperty("stream").GetBoolean().Should().BeTrue();
        requestJson.GetProperty("reasoning").GetProperty("enabled").GetBoolean().Should().BeTrue();

        var messages = requestJson.GetProperty("messages").EnumerateArray().ToList();
        messages.Should().HaveCount(1);
        messages[0].GetProperty("role").GetString().Should().Be("user");
        messages[0].GetProperty("content").GetString().Should().Contain(cardData);
    }

    #endregion

    #region Обработка SSE формата

    [Fact]
    public async Task GetCompletionAsync_ShouldHandleOpenRouterProcessingMessages()
    {
        var cardData = "test";
        var sseResponse =
            ": OPENROUTER PROCESSING\n\n" +
            "data: {\"choices\":[{\"delta\":{\"content\":\"Первый\"}}]}\n\n" +
            ": OPENROUTER PROCESSING\n\n" +
            "data: {\"choices\":[{\"delta\":{\"content\":\"Второй\"}}]}\n\n" +
            "data: [DONE]\n\n";

        SetupHttpResponse(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(sseResponse, Encoding.UTF8, "text/event-stream")
        });

        var result = _aiService.GetCompletionAsync(cardData, CancellationToken.None);
        var chunks = new List<string>();

        await foreach (var chunk in result)
        {
            chunks.Add(chunk);
        }

        chunks.Should().HaveCount(2);
        chunks.Should().Equal("Первый", "Второй");
    }

    [Fact]
    public async Task GetCompletionAsync_ShouldStopWhenDoneReceived()
    {
        var cardData = "test";
        var sseResponse =
            "data: {\"choices\":[{\"delta\":{\"content\":\"Чанк1\"}}]}\n\n" +
            "data: {\"choices\":[{\"delta\":{\"content\":\"Чанк2\"}}]}\n\n" +
            "data: [DONE]\n\n" +
            "data: {\"choices\":[{\"delta\":{\"content\":\"Это не должно обработаться\"}}]}\n\n";

        SetupHttpResponse(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(sseResponse, Encoding.UTF8, "text/event-stream")
        });

        var result = _aiService.GetCompletionAsync(cardData, CancellationToken.None);
        var chunks = new List<string>();

        await foreach (var chunk in result)
        {
            chunks.Add(chunk);
        }

        chunks.Should().HaveCount(2);
        chunks.Should().Equal("Чанк1", "Чанк2");
    }

    [Fact]
    public async Task GetCompletionAsync_ShouldSkipEmptyLines()
    {
        var cardData = "test";
        var sseResponse =
            "\n\n" +
            "data: {\"choices\":[{\"delta\":{\"content\":\"Чанк1\"}}]}\n\n" +
            "\n" +
            "data: {\"choices\":[{\"delta\":{\"content\":\"Чанк2\"}}]}\n\n" +
            "   \n" +
            "data: [DONE]\n\n";

        SetupHttpResponse(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(sseResponse, Encoding.UTF8, "text/event-stream")
        });

        var result = _aiService.GetCompletionAsync(cardData, CancellationToken.None);
        var chunks = new List<string>();

        await foreach (var chunk in result)
        {
            chunks.Add(chunk);
        }

        chunks.Should().HaveCount(2);
        chunks.Should().Equal("Чанк1", "Чанк2");
    }

    [Fact]
    public async Task GetCompletionAsync_ShouldHandleDeltaWithoutContent()
    {
        var cardData = "test";
        var sseResponse =
            "data: {\"choices\":[{\"delta\":{\"role\":\"assistant\"}}]}\n\n" +
            "data: {\"choices\":[{\"delta\":{\"content\":\"Реальный контент\"}}]}\n\n" +
            "data: [DONE]\n\n";

        SetupHttpResponse(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(sseResponse, Encoding.UTF8, "text/event-stream")
        });

        var result = _aiService.GetCompletionAsync(cardData, CancellationToken.None);
        var chunks = new List<string>();

        await foreach (var chunk in result)
        {
            chunks.Add(chunk);
        }

        chunks.Should().HaveCount(1);
        chunks.Should().Equal("Реальный контент");
    }

    #endregion

    #region Обработка ошибок

    [Fact]
    public async Task GetCompletionAsync_ShouldHandleMalformedJson()
    {
        var cardData = "test";
        var sseResponse =
            "data: {malformed json\n\n" +
            "data: {\"choices\":[{\"delta\":{\"content\":\"Нормальный чанк\"}}]}\n\n" +
            "data: [DONE]\n\n";

        SetupHttpResponse(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(sseResponse, Encoding.UTF8, "text/event-stream")
        });

        var result = _aiService.GetCompletionAsync(cardData, CancellationToken.None);
        var chunks = new List<string>();

        await foreach (var chunk in result)
        {
            chunks.Add(chunk);
        }

        chunks.Should().HaveCount(1);
        chunks.Should().Equal("Нормальный чанк");
    }

    [Fact]
    public async Task GetCompletionAsync_ShouldHandleEmptyResponse()
    {
        var cardData = "test";

        SetupHttpResponse(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("", Encoding.UTF8, "text/event-stream")
        });

        var result = _aiService.GetCompletionAsync(cardData, CancellationToken.None);
        var chunks = new List<string>();

        await foreach (var chunk in result)
        {
            chunks.Add(chunk);
        }

        chunks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCompletionAsync_ShouldHandleHttpError()
    {
        var cardData = "test";

        SetupHttpResponse(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Content = new StringContent("Server Error")
        });

        var result = _aiService.GetCompletionAsync(cardData, CancellationToken.None);
        var chunks = new List<string>();

        await foreach (var chunk in result)
        {
            chunks.Add(chunk);
        }

        chunks.Should().BeEmpty();
    }

    #endregion

    #region Отмена операций

    [Fact]
    public async Task GetCompletionAsync_ShouldRespectCancellationToken()
    {
        var cardData = "test";
        var cts = new CancellationTokenSource();
        var chunks = new List<string>();

        var sseResponse =
            "data: {\"choices\":[{\"delta\":{\"content\":\"Чанк1\"}}]}\n\n" +
            "data: {\"choices\":[{\"delta\":{\"content\":\"Чанк2\"}}]}\n\n" +
            "data: {\"choices\":[{\"delta\":{\"content\":\"Чанк3\"}}]}\n\n";

        SetupHttpResponse(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(sseResponse, Encoding.UTF8, "text/event-stream")
        });

        cts.CancelAfter(50);

        var result = _aiService.GetCompletionAsync(cardData, cts.Token);

        var exception = await Record.ExceptionAsync(async () =>
        {
            await foreach (var chunk in result)
            {
                chunks.Add(chunk);
            }
        });

        exception.Should().BeOfType<OperationCanceledException>();
    }

    #endregion

    #region Вспомогательные методы

    private void SetupHttpResponse(HttpResponseMessage response)
    {
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    private HttpResponseMessage CreateMockSSEResponse(string[] chunks)
    {
        var sb = new StringBuilder();

        foreach (var chunk in chunks)
        {
            var jsonChunk = new
            {
                choices = new[]
                {
                    new
                    {
                        delta = new { content = chunk }
                    }
                }
            };

            sb.AppendLine($"data: {JsonSerializer.Serialize(jsonChunk)}");
            sb.AppendLine();
        }

        sb.AppendLine("data: [DONE]");
        sb.AppendLine();

        return new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(sb.ToString(), Encoding.UTF8, "text/event-stream")
        };
    }

    #endregion
}
