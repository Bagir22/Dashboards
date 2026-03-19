namespace Infrastructure.ETLPipeline.Extract.ApiAuth
{
    public interface IApiAuthRequest
    {
        public Task<string> GetTokenAsync();
    }
}
