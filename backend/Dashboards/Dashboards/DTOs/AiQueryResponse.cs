namespace Dashboards.DTOs
{
    public class AiQueryResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? Analysis { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
