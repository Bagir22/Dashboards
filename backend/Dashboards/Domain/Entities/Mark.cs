namespace Domain.Entities
{
    public class Mark
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Value { get; set; }
        public bool IsGoodMark { get; set; }
    }
}
