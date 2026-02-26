namespace Infrastructure.ETLPipeline.Extract.Order
{
    public interface IOrderRequest
    {
        public Task<List<OrderResponse>> GetAllOrdersAsync(string token, string studentId);
    }
}
