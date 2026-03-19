namespace Domain.Entities
{
    public class OrderCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Order> Orders { get; set; } = new();
    }
}
