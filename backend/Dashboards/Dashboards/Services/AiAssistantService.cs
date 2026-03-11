using System.Text;
using System.Text.Json;
using Dashboards.DTOs;
using Dashboards.Prompts;
using Application.Contracts;
using Application.Features.StudentDynamic;
using Application.Features.FacultyDistribution;
using Application.Features.GenderDistribution;
using Application.Features.TopCitizenship;
using Application.Features.TopEducationProgram;
using Application.Filters;
using MediatR;

namespace Dashboards.Services
{
    public class AiAssistantService : IAiAssistantService
    {
        private readonly IMediator _mediator;
        private readonly IMetabaseService _metabaseService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AiAssistantService> _logger;
        private readonly string _llmUrl = "http://localhost:11434/api/generate";

        public AiAssistantService(
            IMediator mediator,
            IMetabaseService metabaseService,
            IHttpClientFactory httpClientFactory,
            ILogger<AiAssistantService> logger)
        {
            _mediator = mediator;
            _metabaseService = metabaseService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<AiQueryResponse> ProcessQueryAsync(AiQueryRequest request)
        {
            try
            {
                string? dataContext = null;

                if (request.Query.Contains("динамик") || request.Query.Contains("студент"))
                {
                    dataContext = await GetStudentDynamicsData(request.Filters);
                }
                else if (request.Query.Contains("факультет") || request.Query.Contains("институт"))
                {
                    dataContext = await GetFacultyDistributionData(request.Filters);
                }

                var prompt = AnalysisPrompt.GeneratePrompt(request.Query, dataContext);
                var analysis = await CallLocalLLM(prompt);

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

        public async Task<AiQueryResponse> AnalyzeDashboardAsync(AnalyzeDashboardRequest request)
        {
            try
            {
                _logger.LogInformation("Starting full dashboard analysis for {DashboardId}", request.DashboardId);

                var dashboardData = await FetchDashboardDataFromMetabase(request.DashboardId);
                var dbData = await FetchDatabaseStats();

                var analysisContext = new
                {
                    DashboardName = request.DashboardName,
                    MetabaseData = dashboardData,
                    DatabaseStats = dbData,
                    AnalysisTimestamp = DateTime.Now
                };

                var prompt = GenerateFullAnalysisPrompt(analysisContext);
                var analysis = await CallLocalLLM(prompt);

                return new AiQueryResponse
                {
                    Success = true,
                    Message = "Анализ дашборда выполнен",
                    Analysis = analysis
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing dashboard {DashboardId}", request.DashboardId);
                return new AiQueryResponse
                {
                    Success = false,
                    ErrorMessage = "Не удалось выполнить анализ дашборда. Попробуйте позже."
                };
            }
        }

        public async Task<AiQueryResponse> AnalyzeMetricAsync(AnalyzeMetricRequest request)
        {
            try
            {
                var data = request.MetricId switch
                {
                    "overview" => await GetOverviewData(),
                    "dynamics" => await GetStudentDynamicsData(new FilterParams()),
                    "faculty" => await GetFacultyDistributionData(new FilterParams()),
                    "gender" => await GetGenderDistributionData(new FilterParams()),
                    "citizenship" => await GetTopCitizenshipData(new FilterParams()),
                    "education" => await GetEducationProgramsData(new FilterParams()),
                    _ => null
                };

                if (data == null)
                {
                    return new AiQueryResponse
                    {
                        Success = false,
                        ErrorMessage = $"Неизвестная метрика: {request.MetricId}"
                    };
                }

                var prompt = $@"
Ты - аналитический помощник университета.
Проанализируй следующие данные по метрике '{request.MetricName}':

{data}

Дай краткий, содержательный анализ на русском языке.
Выдели ключевые тенденции и интересные наблюдения.";

                var analysis = await CallLocalLLM(prompt);

                return new AiQueryResponse
                {
                    Success = true,
                    Analysis = analysis
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing metric {MetricId}", request.MetricId);
                return new AiQueryResponse
                {
                    Success = false,
                    ErrorMessage = "Не удалось проанализировать метрику"
                };
            }
        }

        private async Task<object?> FetchDashboardDataFromMetabase(string dashboardId)
        {
            if (int.TryParse(dashboardId, out int metabaseId))
            {
                return await _metabaseService.GetCardDataJsonAsync(metabaseId);
            }
            return null;
        }

        private async Task<object> FetchDatabaseStats()
        {
            var dynamics = await _mediator.Send(new GetStudentDynamicsQuery(new FilterParams()));
            var facultyDist = await _mediator.Send(new GetFacultyDistributionQuery(new FilterParams()));
            var genderDist = await _mediator.Send(new GetGenderDistributionQuery(new FilterParams()));

            // ИСПРАВЛЕНО: используем рефлексию для получения Count
            var totalStudents = dynamics?.Sum(d => 
            {
                var count = d.GetType().GetProperty("Count")?.GetValue(d) as int? ?? 0;
                return count;
            }) ?? 0;

            return new
            {
                TotalStudents = totalStudents,
                FacultyCount = facultyDist?.Count ?? 0,
                GenderStats = genderDist
            };
        }

        private string GenerateFullAnalysisPrompt(object context)
        {
            var json = JsonSerializer.Serialize(context, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return $@"
Ты - эксперт по анализу данных университета.
Проведи комплексный анализ дашборда '{((dynamic)context).DashboardName}'.

Данные для анализа:
{json}

Требования к анализу:
1. Общая статистика (количество студентов, динамика)
2. Структура по факультетам
3. Гендерное распределение
4. Гражданство студентов
5. Образовательные программы
6. Ключевые выводы и рекомендации

Формат ответа: структурированный текст на русском языке с заголовками разделов.";
        }

        private async Task<string> GetStudentDynamicsData(FilterParams? filters)
        {
            try
            {
                var query = new GetStudentDynamicsQuery(filters ?? new FilterParams());
                var result = await _mediator.Send(query);

                if (result == null || !result.Any())
                    return "Нет данных по динамике студентов за указанный период.";

                var data = result.Select(r =>
                {
                    var date = r.GetType().GetProperty("Date")?.GetValue(r)?.ToString() ?? "Неизвестная дата";
                    var count = r.GetType().GetProperty("Count")?.GetValue(r)?.ToString() ?? "0";
                    return $"{date}: {count} студентов";
                });

                return string.Join("\n", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student dynamics");
                return "Ошибка при получении данных по динамике студентов.";
            }
        }

        private async Task<string> GetFacultyDistributionData(FilterParams? filters)
        {
            try
            {
                var query = new GetFacultyDistributionQuery(filters ?? new FilterParams());
                var result = await _mediator.Send(query);

                if (result == null || !result.Any())
                    return "Нет данных по распределению по факультетам.";

                var data = result.Select(r =>
                {
                    var faculty = r.GetType().GetProperty("Faculty")?.GetValue(r)?.ToString() ?? "Неизвестный факультет";
                    var total = r.GetType().GetProperty("Total")?.GetValue(r)?.ToString() ?? "0";
                    var budget = r.GetType().GetProperty("Budget")?.GetValue(r)?.ToString() ?? "0";
                    var paid = r.GetType().GetProperty("Paid")?.GetValue(r)?.ToString() ?? "0";
                    return $"{faculty}: всего {total} студентов (бюджет: {budget}, платно: {paid})";
                });

                return string.Join("\n", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting faculty distribution");
                return "Ошибка при получении данных по факультетам.";
            }
        }

        private async Task<string> GetGenderDistributionData(FilterParams filters)
        {
            var result = await _mediator.Send(new GetGenderDistributionQuery(filters));
            if (result == null || !result.Any()) 
                return "Нет данных по гендерному распределению";

            var total = result.Sum(r => 
            {
                var count = r.GetType().GetProperty("Count")?.GetValue(r) as int? ?? 0;
                return count;
            });

            var data = result.Select(r =>
            {
                var gender = r.GetType().GetProperty("Gender")?.GetValue(r)?.ToString() ?? "Неизвестно";
                var count = r.GetType().GetProperty("Count")?.GetValue(r) as int? ?? 0;
                var percentage = total > 0 ? (count * 100.0 / total) : 0;
                return $"{gender}: {count} студентов ({percentage:F1}%)";
            });

            return string.Join("\n", data);
        }

        private async Task<string> GetTopCitizenshipData(FilterParams filters)
        {
            var result = await _mediator.Send(new GetTopCitizenshipQuery(filters));
            if (result == null || !result.Any()) 
                return "Нет данных по гражданству";

            var total = result.Sum(r => 
            {
                var count = r.GetType().GetProperty("Count")?.GetValue(r) as int? ?? 0;
                return count;
            });

            var data = result.Select(r =>
            {
                var country = r.GetType().GetProperty("Country")?.GetValue(r)?.ToString() ?? "Неизвестно";
                var count = r.GetType().GetProperty("Count")?.GetValue(r) as int? ?? 0;
                var percentage = total > 0 ? (count * 100.0 / total) : 0;
                return $"{country}: {count} студентов ({percentage:F1}%)";
            });

            return string.Join("\n", data);
        }

        private async Task<string> GetEducationProgramsData(FilterParams filters)
        {
            var result = await _mediator.Send(new GetTopEducationProgramQuery(filters));
            if (result == null || !result.Any()) 
                return "Нет данных по образовательным программам";

            var data = result.Select(r =>
            {
                var program = r.GetType().GetProperty("EducationProgram")?.GetValue(r)?.ToString() 
                              ?? r.GetType().GetProperty("Name")?.GetValue(r)?.ToString() 
                              ?? "Неизвестно";
                var count = r.GetType().GetProperty("Count")?.GetValue(r) as int? ?? 0;
                return $"{program}: {count} студентов";
            });

            return string.Join("\n", data);
        }

        private async Task<string> GetOverviewData()
        {
            var dynamics = await _mediator.Send(new GetStudentDynamicsQuery(new FilterParams()));
            var faculty = await _mediator.Send(new GetFacultyDistributionQuery(new FilterParams()));
            
            var totalStudents = dynamics?.Sum(d => 
            {
                var count = d.GetType().GetProperty("Count")?.GetValue(d) as int? ?? 0;
                return count;
            }) ?? 0;
            
            return $"Всего студентов: {totalStudents}\n" +
                   $"Факультетов: {faculty?.Count ?? 0}";
        }

        private async Task<string> CallLocalLLM(string prompt)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var requestBody = new
                {
                    model = "qwen2.5:3b",
                    prompt = $"Ты - аналитический помощник университета. Отвечай кратко, по делу, на русском языке.\n\n{prompt}",
                    stream = false,
                    temperature = 0.7,
                    max_tokens = 1000
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