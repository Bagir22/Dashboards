namespace Application.Features.AverageBallByInstitutes
{
    public class AverageBallByInstitutesResponseDto
    {
        public DateTime Date { get; set; }
        public required string Institute { get; set; }
        public double AverageBall { get; set; }
    }
}
