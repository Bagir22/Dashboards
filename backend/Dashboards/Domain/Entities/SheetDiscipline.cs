namespace Domain.Entities
{
    public class SheetDiscipline
    {
        public Guid Id { get; set; }
        public int Retake { get; set; }
        public DateTime? MarkDate { get; set; }
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public Guid DisciplineId { get; set; }
        public Discipline Discipline { get; set; } = null!;
        public Guid SemesterId { get; set; }
        public Semester Semester { get; set; } = null!;
        public Guid? MarkId { get; set; }
        public Mark? Mark { get; set; }
    }
}
