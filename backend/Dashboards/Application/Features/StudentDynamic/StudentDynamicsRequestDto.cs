namespace Application.Features.StudentDynamic
{
    public class StudentDynamicsRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? StudyFormId { get; set; }
        public Guid? FacultyId { get; set; }
        public int? CourseNum { get; set; }
        public Guid? BenefitId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? EducationProgrammId { get; set; }
        public Guid? EducationStandartId { get; set; }
        public int? Gender { get; set; }
        public Guid? CitizenshipId { get; set; }
    }
}
