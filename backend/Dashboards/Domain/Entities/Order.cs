namespace Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }

        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public OrderCategory Category { get; set; } = null!;
    }
}
