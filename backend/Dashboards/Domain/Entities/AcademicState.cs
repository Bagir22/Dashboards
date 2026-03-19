namespace Domain.Entities
{
    public class AcademicState
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ContingentStudent> Students { get; set; } = new();
    }
}
