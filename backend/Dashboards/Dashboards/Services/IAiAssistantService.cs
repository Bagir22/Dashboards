using Dashboards.DTOs;

namespace Dashboards.Services
{
    public interface IAiAssistantService
    {
        Task<AiQueryResponse> ProcessQueryAsync(AiQueryRequest request);
    }
}
