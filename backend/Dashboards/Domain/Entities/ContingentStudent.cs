namespace Domain.Entities
{
    public class ContingentStudent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AcademicStateId { get; set; }
        public AcademicState AcademicState { get; set; } = null!;
        public Guid? AddressStateId { get; set; }
        public AddressState? AddressState { get; set; }

        public int? Course { get; set; }
        public double? Ball { get; set; }
        public string? Budget { get; set; }
        public DateTime ContingentDate { get; set; }

        public Guid StudentExternalId { get; set; }
        public Student Student { get; set; } = null!;
    }
}
