namespace Domain.Entities
{
    public class AddressState
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ContingentStudent> Students { get; set; } = new();
    }
}
