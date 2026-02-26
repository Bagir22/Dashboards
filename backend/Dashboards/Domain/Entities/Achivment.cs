namespace Domain.Entities
{
    public class Achivment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime BeginDate { get; set; }

        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public AchivmentCategory Category { get; set; } = null!;
    }
}
