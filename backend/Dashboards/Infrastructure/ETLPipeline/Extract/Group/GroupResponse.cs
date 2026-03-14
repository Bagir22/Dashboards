using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Group
{
    public class GroupResponse
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("branch")]
        public required GroupBranchResponse Branch { get; set; }
        [JsonPropertyName("semesters")]
        public required List<SemesterResponse> Semesters { get; set; }
    }

    public class GroupBranchResponse
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }

    public class SemesterResponse
    {
        [JsonPropertyName("semester")]
        public required int Semester { get; set; }
        [JsonPropertyName("begin")]
        public required DateTime Begin { get; set; }
        [JsonPropertyName("end")]
        public required DateTime End { get; set; }
    }
}
