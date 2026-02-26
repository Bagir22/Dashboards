namespace Domain.Entities
{
    public class Student
    {
        public Guid Id { get; set; }

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
        public Guid? TrainingLevelId { get; set; }
        public TrainingLevel? TrainingLevel { get; set; }
        public Guid? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public Guid? GroupId { get; set; }

        public int Gender { get; set; }
        public string Fio {  get; set; } = null!;
        public int AdmissionYear { get; set; }

        public List<Achivment> Achivments { get; set; } = new();
        public List<ContingentStudent> ContingentStudents { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
    }
}
