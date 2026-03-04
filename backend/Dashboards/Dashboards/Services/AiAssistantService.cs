using System.Text;
using System.Text.Json;
using Dashboards.DTOs;
using Dashboards.Prompts;
using Application.Contracts;
using Application.Features.StudentDynamic;
using Application.Features.FacultyDistribution;
using Application.Filters;
using MediatR;

namespace Dashboards.Services
{
    public class AiAssistantService : IAiAssistantService
    {
        private readonly IMediator _mediator;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AiAssistantService> _logger;
        private readonly string _llmUrl = "http://localhost:11434/api/generate"; // Ollama

        public AiAssistantService(
            IMediator mediator,
            IHttpClientFactory httpClientFactory,
            ILogger<AiAssistantService> logger)
        {
            _mediator = mediator;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<AiQueryResponse> ProcessQueryAsync(AiQueryRequest request)
        {
            try
            {
                // 1. Определяем тип запроса и получаем данные
                string? dataContext = null;

                if (request.Query.Contains("динамик") || request.Query.Contains("студент"))
                {
                    dataContext = await GetStudentDynamicsData(request.Filters);
                }
                else if (request.Query.Contains("факультет") || request.Query.Contains("институт"))
                {
                    dataContext = await GetFacultyDistributionData(request.Filters);
                }

                // 2. Формируем промпт
                var prompt = AnalysisPrompt.GeneratePrompt(request.Query, dataContext);

                // 3. Отправляем запрос к LLM
                var analysis = await CallLocalLLM(prompt);

                // 4. Возвращаем ответ
                return new AiQueryResponse
                {
                    Message = "Анализ выполнен",
                    Analysis = analysis,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AI query");
                return new AiQueryResponse
                {
                    Success = false,
                    ErrorMessage = "Не удалось выполнить анализ. Попробуйте позже."
                };
            }
        }

        private async Task<string?> GetStudentDynamicsData(FilterParams? filters)
        {
            try
            {
                var query = new GetStudentDynamicsQuery(filters ?? new FilterParams());
                var result = await _mediator.Send(query);

                if (result == null || !result.Any())
                {
                    return "Нет данных по динамике студентов за указанный период.";
                }

                // Предполагаем, что в ответе есть свойства Date и Count
                // Если название свойства другое, нужно заменить на правильное
                var data = result.Select(r => 
                {
                    // Используем рефлексию для получения значений, если не знаем точных названий свойств
                    var date = r.GetType().GetProperty("Date")?.GetValue(r)?.ToString() ?? "Неизвестная дата";
                    var count = r.GetType().GetProperty("Count")?.GetValue(r)?.ToString() ?? "0";
                    return $"{date}: {count} студентов";
                }).ToList();
                
                return string.Join("\n", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student dynamics");
                return "Ошибка при получении данных по динамике студентов.";
            }
        }

        private async Task<string?> GetFacultyDistributionData(FilterParams? filters)
        {
            try
            {
                var query = new GetFacultyDistributionQuery(filters ?? new FilterParams());
                var result = await _mediator.Send(query);

                if (result == null || !result.Any())
                {
                    return "Нет данных по распределению по факультетам.";
                }

                var data = result.Select(r => 
                    $"{r.Faculty}: всего {r.Total} студентов (бюджет: {r.Budget}, платно: {r.Paid})"
                ).ToList();
                
                return string.Join("\n", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting faculty distribution");
                return "Ошибка при получении данных по факультетам.";
            }
        }

        private async Task<string> CallLocalLLM(string prompt)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var requestBody = new
                {
                    model = "qwen2.5:3b",
                    prompt = $"{AnalysisPrompt.GetSystemPrompt()}\n\n{prompt}",
                    stream = false,
                    temperature = 0.7,
                    max_tokens = 500
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(_llmUrl, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OllamaResponse>(responseBody);

                return result?.response ?? "Не удалось получить ответ от модели";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling LLM");
                return "Ошибка соединения с моделью AI. Убедитесь, что Ollama запущена.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling local LLM");
                return "Ошибка при обращении к AI модели.";
            }
        }

        private class OllamaResponse
        {
            public string response { get; set; } = string.Empty;
        }
    }
}