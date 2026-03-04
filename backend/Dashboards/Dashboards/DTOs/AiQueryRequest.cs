using Application.Filters;

namespace Dashboards.DTOs
{
    public class AiQueryRequest
    {
        public string Query { get; set; } = string.Empty;
        public string? DashboardId { get; set; }
        public FilterParams? Filters { get; set; }
    }
}
