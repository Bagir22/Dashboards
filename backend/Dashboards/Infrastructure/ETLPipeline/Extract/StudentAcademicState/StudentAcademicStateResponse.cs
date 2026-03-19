using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.StudentAcademicState
{
    public class StudentAcademicStateResponse
    {
        [JsonPropertyName( "dictStudentAcademicStateExternalId" )]
        public required string DictStudentAcademicStateExternalId { get; set; }
        [JsonPropertyName( "academicStateName" )]
        public required string AcademicStateName { get; set; }
    }
}
