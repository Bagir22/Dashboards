namespace Application.Features.StudentDynamic
{
    public class StudentDynamicsResponseDto
    {
        public DateTime Date { get; set; }
        public int Studying { get; set; }
        public int Awaiting { get; set; }
        public int OnLeave { get; set; }
        public int Expelled { get; set; }
    }
}