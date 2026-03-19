using Application.Contracts;
using Dashboards.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;
using Xunit;

namespace Tests.WebApi.Controllers;

public class AnalysisControllerTests
{
    private readonly Mock<IAIService> _mockAIService;
    private readonly Mock<IMetabaseService> _mockMetabaseService;
    private readonly AnalysisController _controller;

    public AnalysisControllerTests()
    {
        _mockAIService = new Mock<IAIService>();
        _mockMetabaseService = new Mock<IMetabaseService>();
        _controller = new AnalysisController(_mockAIService.Object, _mockMetabaseService.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    #region Успешные сценарии

    [Fact]
    public async Task AnalyseCard_ShouldReturnAIAnalysis_WhenCardIdValid()
    {
        var cardId = 123;
        var expectedChunks = new[] { "Анализ:", "рост на 5%", "аномалий не обнаружено" };

        Environment.SetEnvironmentVariable("METABASE_USER", "test@test.com");
        Environment.SetEnvironmentVariable("METABASE_PASS", "password");

        _mockMetabaseService.Setup(m => m.AuthenticateAsync("test@test.com", "password"))
            .Returns(Task.CompletedTask);

        _mockMetabaseService.Setup(m => m.GetCardDataJsonAsync(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("{\"data\": [{\"value\": 100}]}");

        _mockAIService.Setup(s => s.GetCompletionAsync("{\"data\": [{\"value\": 100}]}", It.IsAny<CancellationToken>()))
            .Returns(GetTestStream(expectedChunks));

        await _controller.AnalyseCard(cardId, CancellationToken.None);

        var responseBody = await GetResponseBodyAsync(_controller.Response.Body);

        responseBody.Should().Contain("data: Анализ:");
        responseBody.Should().Contain("data: рост на 5%");
        responseBody.Should().Contain("data: аномалий не обнаружено");
        responseBody.Should().Contain("data: [DONE]");
    }

    [Fact]
    public async Task AnalyseCard_ShouldPassCardDataToAIService()
    {
        var cardId = 456;
        var metabaseData = "{\"students\": 1250, \"trend\": \"+8%\"}";

        Environment.SetEnvironmentVariable("METABASE_USER", "test@test.com");
        Environment.SetEnvironmentVariable("METABASE_PASS", "password");

        _mockMetabaseService.Setup(m => m.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockMetabaseService.Setup(m => m.GetCardDataJsonAsync(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(metabaseData);

        _mockAIService.Setup(s => s.GetCompletionAsync(metabaseData, It.IsAny<CancellationToken>()))
            .Returns(GetTestStream(new[] { "test" }));

        await _controller.AnalyseCard(cardId, CancellationToken.None);

        _mockMetabaseService.Verify(m => m.GetCardDataJsonAsync(cardId, It.IsAny<CancellationToken>()), Times.Once);
        _mockAIService.Verify(s => s.GetCompletionAsync(metabaseData, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AnalyseCard_ShouldUseCorrectSSEFormat()
    {
        var cardId = 123;
        var chunks = new[] { "chunk1", "chunk2" };

        Environment.SetEnvironmentVariable("METABASE_USER", "test@test.com");
        Environment.SetEnvironmentVariable("METABASE_PASS", "password");

        _mockMetabaseService.Setup(m => m.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _mockMetabaseService.Setup(m => m.GetCardDataJsonAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("{}");
        _mockAIService.Setup(s => s.GetCompletionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(GetTestStream(chunks));

        await _controller.AnalyseCard(cardId, CancellationToken.None);

        _controller.Response.Headers["Content-Type"].ToString().Should().Be("text/event-stream");
        _controller.Response.Headers["Cache-Control"].ToString().Should().Be("no-cache");

        var responseBody = await GetResponseBodyAsync(_controller.Response.Body);
        var lines = responseBody.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        lines.Should().Contain("data: chunk1");
        lines.Should().Contain("data: chunk2");
        lines.Should().Contain("data: [DONE]");
    }

    [Fact]
    public async Task AnalyseCards_ShouldCombineMultipleCardsData()
    {
        var cardIds = new[] { 1, 2 };
        var card1Data = "{\"card\": 1}";
        var card2Data = "{\"card\": 2}";
        var combinedData = card1Data + card2Data;

        Environment.SetEnvironmentVariable("METABASE_USER", "test@test.com");
        Environment.SetEnvironmentVariable("METABASE_PASS", "password");

        _mockMetabaseService.Setup(m => m.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockMetabaseService.SetupSequence(m => m.GetCardDataJsonAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(card1Data)
            .ReturnsAsync(card2Data);

        _mockAIService.Setup(s => s.GetCompletionAsync(combinedData, It.IsAny<CancellationToken>()))
            .Returns(GetTestStream(new[] { "combined analysis" }));

        await _controller.AnalyseCards(cardIds, CancellationToken.None);

        _mockMetabaseService.Verify(m => m.GetCardDataJsonAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _mockAIService.Verify(s => s.GetCompletionAsync(combinedData, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Обработка ошибок

    [Fact]
    public async Task AnalyseCard_ShouldHandleMetabaseError()
    {
        var cardId = 123;

        Environment.SetEnvironmentVariable("METABASE_USER", "test@test.com");
        Environment.SetEnvironmentVariable("METABASE_PASS", "password");

        _mockMetabaseService.Setup(m => m.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _mockMetabaseService.Setup(m => m.GetCardDataJsonAsync(cardId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Metabase API unavailable"));

        await _controller.AnalyseCard(cardId, CancellationToken.None);

        var responseBody = await GetResponseBodyAsync(_controller.Response.Body);
        responseBody.Should().Contain("data: Error: Metabase API unavailable");
        _mockAIService.Verify(s => s.GetCompletionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AnalyseCard_ShouldHandleAIServiceError()
    {
        var cardId = 123;

        Environment.SetEnvironmentVariable("METABASE_USER", "test@test.com");
        Environment.SetEnvironmentVariable("METABASE_PASS", "password");

        _mockMetabaseService.Setup(m => m.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _mockMetabaseService.Setup(m => m.GetCardDataJsonAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("{}");

        _mockAIService.Setup(s => s.GetCompletionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception("AI service timeout"));

        await _controller.AnalyseCard(cardId, CancellationToken.None);

        var responseBody = await GetResponseBodyAsync(_controller.Response.Body);
        responseBody.Should().Contain("data: Error: AI service timeout");
    }

    [Fact]
    public async Task AnalyseCard_ShouldHandleMissingCredentials()
    {
        var cardId = 123;
        Environment.SetEnvironmentVariable("METABASE_USER", null);
        Environment.SetEnvironmentVariable("METABASE_PASS", null);

        await _controller.AnalyseCard(cardId, CancellationToken.None);

        var responseBody = await GetResponseBodyAsync(_controller.Response.Body);
        responseBody.Should().Contain("data: Error: Metabase credentials are not configured");
    }

    #endregion

    #region Отмена операций

    [Fact]
    public async Task AnalyseCard_ShouldRespectCancellationToken()
    {
        var cardId = 123;
        var cts = new CancellationTokenSource();

        Environment.SetEnvironmentVariable("METABASE_USER", "test@test.com");
        Environment.SetEnvironmentVariable("METABASE_PASS", "password");

        _mockMetabaseService.Setup(m => m.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _mockMetabaseService.Setup(m => m.GetCardDataJsonAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("{}");

        _mockAIService.Setup(s => s.GetCompletionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(GetSlowStream(cts.Token));

        cts.CancelAfter(50);
        await _controller.AnalyseCard(cardId, cts.Token);
    }

    #endregion

    #region Вспомогательные методы

    private async IAsyncEnumerable<string> GetTestStream(string[] chunks)
    {
        foreach (var chunk in chunks)
        {
            yield return chunk;
            await Task.Delay(10);
        }
    }

    private async IAsyncEnumerable<string> GetSlowStream(CancellationToken token)
    {
        for (int i = 0; i < 10; i++)
        {
            token.ThrowIfCancellationRequested();
            yield return i.ToString();
            await Task.Delay(100, token);
        }
    }

    private async Task<string> GetResponseBodyAsync(Stream stream)
    {
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    #endregion
}
