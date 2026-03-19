using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Faculty
{
    public class FacultyResponse
    {
        [JsonPropertyName( "facultyExternalId" )]
        public required string FacultyExternalId { get; set; }
        [JsonPropertyName( "name" )]
        public required string Name { get; set; }
    }
}
