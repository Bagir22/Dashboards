namespace Infrastructure.ETLPipeline.Extract.AchivmentCategory
{
    public interface IAchivmentCategoryRequest
    {
        public Task<List<AchivmentCategoryResponse>> GetAllAchivmentCategoriesAsync(string token);
    }
}
