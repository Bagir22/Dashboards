using FluentAssertions;
using Infrastructure.Analysis.Services.AIService;
using Xunit;

namespace Tests.Application.Features;

public class PromptTests
{
    #region Базовые тесты формирования промпта

    [Fact]
    public void GetFromCardData_ShouldReturnNonEmptyString_WhenCardDataProvided()
    {
        var cardData = "{\"data\": [{\"value\": 100, \"date\": \"2024-01-01\"}]}";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetFromCardData_ShouldContainAllRequiredSections()
    {
        var cardData = "test data";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().Contain("Краткий анализ:")
              .And.Contain("Ключевые наблюдения:")
              .And.Contain("Данные дашборда:");
    }

    [Fact]
    public void GetFromCardData_ShouldIncludeProvidedCardData()
    {
        var cardData = "{\"students\": 1250, \"trend\": \"+5%\"}";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().Contain(cardData);
    }

    #endregion

    #region Тесты структуры и инструкций для AI

    [Fact]
    public void GetFromCardData_ShouldIncludeRoleInstructionForAI()
    {
        var cardData = "test";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().Contain("Ты — аналитик данных");
    }

    [Fact]
    public void GetFromCardData_ShouldSpecifyAnalysisGoals()
    {
        var cardData = "test";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().Contain("Определи основную метрику")
              .And.Contain("Найди ключевые изменения")
              .And.Contain("Отметь заметные скачки");
    }

    [Fact]
    public void GetFromCardData_ShouldIncludeConstraintsForAI()
    {
        var cardData = "test";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().Contain("Не пересказывай все значения")
              .And.Contain("Сосредоточься только на значимых изменениях")
              .And.Contain("Не придумывай данные");
    }

    [Fact]
    public void GetFromCardData_ShouldSpecifyOutputFormat()
    {
        var cardData = "test";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().Contain("Краткий анализ:")
              .And.Contain("Ключевые наблюдения:")
              .And.Contain("- пункт");
    }

    [Fact]
    public void GetFromCardData_ShouldLimitResponseLength()
    {
        var cardData = "test";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().Contain("3–5 предложений");
    }

    #endregion

    #region Тесты обработки граничных случаев

    [Fact]
    public void GetFromCardData_ShouldHandleEmptyString()
    {
        var cardData = "";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().NotBeNull();
        result.Should().Contain("Данные дашборда:");
        result.Should().Contain("Краткий анализ:");
    }

    [Fact]
    public void GetFromCardData_ShouldHandleNullString()
    {
        string cardData = null;
        
        var act = () => Prompt.GetFromCardData(cardData);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetFromCardData_ShouldHandleVeryLargeInput()
    {
        var largeData = new string('x', 100000);
        
        var result = Prompt.GetFromCardData(largeData);
        
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(largeData);
    }

    [Fact]
    public void GetFromCardData_ShouldHandleSpecialCharacters()
    {
        var cardData = "!@#$%^&*()_+{}[]|\"':;?/><.,~`";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().Contain(cardData);
    }

    [Fact]
    public void GetFromCardData_ShouldHandleJsonData()
    {
        var cardData = @"
        {
            ""query_result"": {
                ""data"": {
                    ""rows"": [
                        {""date"": ""2024-01"", ""count"": 150},
                        {""date"": ""2024-02"", ""count"": 165},
                        {""date"": ""2024-03"", ""count"": 182}
                    ]
                }
            }
        }";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().Contain(cardData.Trim());
    }

    #endregion

    #region Тесты реальных сценариев использования

    [Fact]
    public void GetFromCardData_WithStudentData_ShouldFormatCorrectly()
    {
        var studentData = @"
        {
            ""total_students"": 1250,
            ""by_faculty"": [
                {""faculty"": ""ФИиВТ"", ""count"": 450},
                {""faculty"": ""ФИиСТ"", ""count"": 380},
                {""faculty"": ""ФУП"", ""count"": 420}
            ],
            ""trend"": ""+8% к прошлому году""
        }";
        
        var result = Prompt.GetFromCardData(studentData);
        
        result.Should().Contain("Ты — аналитик данных");
        result.Should().Contain("Определи основную метрику");
        result.Should().Contain(studentData);
        result.Should().Contain("Краткий анализ:");
        result.Should().Contain("Ключевые наблюдения:");
    }

    [Fact]
    public void GetFromCardData_WithTimeSeriesData_ShouldInstructTrendAnalysis()
    {
        var timeSeriesData = @"
        {
            ""months"": [""Янв"", ""Фев"", ""Мар"", ""Апр""],
            ""values"": [120, 135, 128, 150]
        }";
        
        var result = Prompt.GetFromCardData(timeSeriesData);
        
        result.Should().Contain("Найди ключевые изменения, тренды");
        result.Should().Contain("Отметь заметные скачки, падения");
    }

    [Fact]
    public void GetFromCardData_WithInsufficientData_ShouldAllowHonestResponse()
    {
        var minimalData = "{}";
        
        var result = Prompt.GetFromCardData(minimalData);
        
        result.Should().Contain("Если данных недостаточно для анализа — укажи это");
    }

    #endregion

    #region Тесты форматирования

    [Fact]
    public void GetFromCardData_ShouldFormatOutputWithProperLineBreaks()
    {
        var cardData = "test";
        
        var result = Prompt.GetFromCardData(cardData);
        
        var lines = result.Split('\n');
        lines.Should().HaveCountGreaterThan(10);
        lines.Should().Contain(line => line.Contains("Краткий анализ:"));
        lines.Should().Contain(line => line.Contains("Ключевые наблюдения:"));
        lines.Should().Contain(line => line.Contains("- пункт"));
    }

    [Fact]
    public void GetFromCardData_ShouldSeparateSectionsWithBlankLines()
    {
        var cardData = "test";
        
        var result = Prompt.GetFromCardData(cardData);
        
        result.Should().Contain("Краткий анализ:\n\n");
        result.Should().MatchRegex(@"Ключевые наблюдения:\s*\n-");
    }

    #endregion

    #region Тесты производительности

    [Fact]
    public void GetFromCardData_ShouldExecuteQuickly()
    {
        var cardData = "test data for performance test";
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var result = Prompt.GetFromCardData(cardData);
        stopwatch.Stop();
        
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetFromCardData_ShouldHandleMultipleCallsEfficiently()
    {
        var cardData = "test";
        var results = new List<string>();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int i = 0; i < 100; i++)
        {
            results.Add(Prompt.GetFromCardData(cardData + i));
        }
        stopwatch.Stop();
        
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
        results.Should().HaveCount(100);
        results.Should().AllSatisfy(r => r.Should().NotBeNullOrEmpty());
    }

    #endregion

    #region Тесты согласованности

    [Fact]
    public void GetFromCardData_ShouldReturnConsistentFormatForSameInput()
    {
        var cardData = "consistent test data";
        
        var result1 = Prompt.GetFromCardData(cardData);
        var result2 = Prompt.GetFromCardData(cardData);
        
        result1.Should().Be(result2);
    }

    [Fact]
    public void GetFromCardData_ShouldIncludeAllInstructionElements()
    {
        var cardData = "test";
        
        var result = Prompt.GetFromCardData(cardData);
        
        var requiredElements = new[]
        {
            "Ты — аналитик данных",
            "краткий текстовый анализ",
            "ключевые изменения",
            "тенденции",
            "основную метрику",
            "Не пересказывай все значения",
            "Не придумывай данные",
            "3–5 предложений",
            "Краткий анализ:",
            "Ключевые наблюдения:"
        };
        
        foreach (var element in requiredElements)
        {
            result.Should().Contain(element);
        }
    }

    #endregion
}