namespace Domain.Entities
{
    public class AchivmentCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Achivment> Achivments { get; set; } = new();
    }
}
