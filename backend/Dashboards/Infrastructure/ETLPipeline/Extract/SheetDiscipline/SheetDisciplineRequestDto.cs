namespace Infrastructure.ETLPipeline.Extract.SheetDiscipline
{
    public class SheetDisciplineRequestDto
    {
        public Guid EduGroupId { get; set; }
        public Guid DisciplineId { get; set; }
        public DateTime MarkDate { get; set; }
        public int Semester { get; set; }
        public bool IsCurrentSemester { get; set; }
    }
}
