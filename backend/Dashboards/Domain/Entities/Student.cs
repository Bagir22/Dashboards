namespace Domain.Entities
{
    public class Student
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StudentExternalId { get; set; }

        public Guid AcademicStateId { get; set; }
        public AcademicState AcademicState { get; set; }
        public Guid? StudyFormId { get; set; }
        public StudyForm? StudyForm { get; set; }
        public Guid CitizenshipId { get; set; }
        public Citizenship Citizenship { get; set; }
        public Guid? FacultyId { get; set; }
        public Faculty? Faculty { get; set; }
        public Guid? EducationProgramId { get; set; }
        public EducationProgram? EducationProgram { get; set; }
        public Guid? EducationStandardId { get; set; }
        public EducationStandard? EducationStandard { get; set; }
        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public Guid? BenefitId { get; set; }
        public Benefit? Benefit { get; set; }
        public Guid? AddressStateId { get; set; }
        public AddressState? AddressState { get; set; }

        public int? Course { get; set; }
        public double? Ball {  get; set; }
        public int Gender { get; set; }
        public string? Budget { get; set; }
        public DateTime ContingentDate { get; set; }
    }
}
