using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.SheetDiscipline
{
    public class SheetDisciplineResponse
    {
        [JsonPropertyName("studentId")]
        public required string StudentId { get; set; }
        [JsonPropertyName("disciplineMark")]
        public DisciplineMark? DisciplineMark { get; set; }
        [JsonPropertyName("retake")]
        public required int Retake { get; set; }
        [JsonPropertyName("markDate")]
        public DateTime? MarkDate { get; set; }
    }

    public class DisciplineMark
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
    }
}
