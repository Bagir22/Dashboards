namespace Domain.Entities
{
    public class Discipline
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public List<SheetDiscipline> SheetDisciplines { get; set; } = new();
        public List<Plan> Plans { get; set; } = new();
    }
}
