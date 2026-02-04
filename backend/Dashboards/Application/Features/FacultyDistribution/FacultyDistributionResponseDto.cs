namespace Application.Features.FacultyDistribution
{
    public class FacultyDistributionResponseDto
    {
        public required string Faculty { get; set; }
        public int Total { get; set; }
        public int Budget { get; set; }
        public int Paid { get; set; }
    }
}
