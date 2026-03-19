namespace Domain.Entities
{
    public class Group
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public List<Semester> Semesters { get; set; } = new();
        public List<Student> Students { get; set; } = new();
    }
}
