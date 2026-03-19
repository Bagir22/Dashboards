namespace Domain.Entities
{
    public class Plan
    {
        public Guid SemesterId { get; set; }
        public Semester Semester { get; set; } = null!;
        public Guid DisciplineId { get; set; }
        public Discipline Discipline { get; set; } = null!;
    }
}
