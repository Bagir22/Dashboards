namespace Domain.Entities
{
    public class BaseDictEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Student> Students { get; set; } = new();
    }
}
