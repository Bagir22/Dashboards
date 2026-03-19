namespace Infrastructure.ETLPipeline.Extract.Student
{
    public class GetStudentsRequestBody
    {
        public required Filter filter { get; set; }
        public required Page page { get; set; }
        public required List<Sort> sort { get; set; }
        public required List<string> fields { get; set; }
    }

    public class Filter
    {
        public required string fio { get; set; }
        public required string orderNumber { get; set; }
        public required List<object> budgetCategories { get; set; }
        public required List<string> trainingLevel { get; set; }
        public required string smartSearch { get; set; }
        public required string institution { get; set; }
        public required string filial { get; set; }
        public required bool isNoTargetTraining { get; set; }
        public required bool export { get; set; }
        public required string contingentDate { get; set; }
        public required string markDate { get; set; }
    }

    public class Page
    {
        public int count { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }

    public class Sort
    {
        public required string dir { get; set; }
        public required string field { get; set; }
    }
}
