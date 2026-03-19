namespace Infrastructure.ETLPipeline.Extract.OrderCategory
{
    public interface IOrderCategoryRequest
    {
        public Task<List<OrderCategoryResponse>> GetAllOrderCategoriesAsync(string token);
    }
}
