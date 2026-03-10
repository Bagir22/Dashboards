namespace Domain.Entities
{
    public class Semester
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Number {  get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; } = null!;
    }
}
