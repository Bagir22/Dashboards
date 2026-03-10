using Dashboards.DTOs;

namespace Dashboards.Services
{
    public interface IAiAssistantService
    {
        Task<AiQueryResponse> ProcessQueryAsync(AiQueryRequest request);
        Task<AiQueryResponse> AnalyzeDashboardAsync(AnalyzeDashboardRequest request);
        Task<AiQueryResponse> AnalyzeMetricAsync(AnalyzeMetricRequest request);
    }
}
