namespace Infrastructure.ETLPipeline.Extract.Group
{
    public interface IGroupRequest
    {
        public Task<List<GroupResponse>> GetAllGroupsAsync(string token);
    }
}
