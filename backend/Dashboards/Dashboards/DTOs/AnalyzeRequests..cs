namespace Dashboards.DTOs
{
    public class AnalyzeDashboardRequest
    {
        public string DashboardId { get; set; } = string.Empty;
        public string DashboardName { get; set; } = string.Empty;
        public string AnalysisType { get; set; } = "full";
        public object? Data { get; set; }
    }

    public class AnalyzeMetricRequest
    {
        public string DashboardId { get; set; } = string.Empty;
        public string DashboardName { get; set; } = string.Empty;
        public string MetricId { get; set; } = string.Empty;
        public string MetricName { get; set; } = string.Empty;
    }
}
