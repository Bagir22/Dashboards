namespace Application.Filters
{
    public class FilterParams
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? StudyFormId { get; set; }
        public Guid? FacultyId { get; set; }
        public int? CourseNum { get; set; }
        public Guid? BenefitId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? EducationProgrammId { get; set; }
        public Guid? EducationStandartId { get; set; }
        public Guid? AcademicStateId { get; set; }
        public int? Gender { get; set; }
        public Guid? CitizenshipId { get; set; }
        public bool? isForeignStudents { get; set; }
    }
}
