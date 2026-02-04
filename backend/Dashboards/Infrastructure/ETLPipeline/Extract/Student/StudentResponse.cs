using System.Text.Json.Serialization;

namespace Infrastructure.ETLPipeline.Extract.Student
{
    public class StudentResponse
    {
        [JsonPropertyName( "studentExternalId" )]
        public required string StudentExternalId { get; set; }
        [JsonPropertyName( "studyForm" )]
        public string? StudyForm { get; set; }
        [JsonPropertyName( "courseNum" )]
        public int? CourseNum { get; set; }
        [JsonPropertyName( "isMaleName" )]
        public required string IsMaleName { get; set; }
        [JsonPropertyName( "citizenship" )]
        public required string Citizenship { get; set; }
        [JsonPropertyName( "facultyName" )]
        public string? FacultyName { get; set; }
        [JsonPropertyName( "addressState" )]
        public required string AddressState { get; set; }
        [JsonPropertyName( "dictStudentAcademicStateId" )]
        public required string AcademicStateId { get; set; }
        [JsonPropertyName( "ball" )]
        public double? Ball { get; set; }
        [JsonPropertyName( "benefit" )]
        public string? Benefit { get; set; }
        [JsonPropertyName( "educationProgramName" )]
        public string? EducationProgramName { get; set; }
        [JsonPropertyName( "educationStandard" )]
        public string? EducationStandard { get; set; }
        [JsonPropertyName( "targetOrganizationName" )]
        public string? TargetOrganizationName { get; set; }
        [JsonPropertyName( "studentBudgetName" )]
        public string? StudentBudgetName { get; set; }
    }

    public class StudentsResponse
    {
        [JsonPropertyName( "items" )]
        public required List<StudentResponse> Items { get; set; }
    }
}
