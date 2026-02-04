using System.ComponentModel.DataAnnotations;

namespace Application.Features.TopOrganization
{
    public class TopOrganizationRequestDto
    {
        [Range( 1900, 2100, ErrorMessage = "Год должен быть в диапазоне от 1900 до 2100." )]
        public int? Year { get; set; } = DateTime.Now.Year;
        [Range( 1, 12, ErrorMessage = "Месяц должен быть в диапазоне от 1 до 12." )]
        public int? Month { get; set; } = DateTime.Now.Month;
        public Guid? StudyFormId { get; set; }
        public Guid? FacultyId { get; set; }
        public int? CourseNum { get; set; }
        public Guid? BenefitId { get; set; }
        public Guid? EducationProgrammId { get; set; }
        public Guid? EducationStandartId { get; set; }
        public Guid? AcademicStateId { get; set; }
        public int? Gender { get; set; }
        public Guid? CitizenshipId { get; set; }
    }
}
