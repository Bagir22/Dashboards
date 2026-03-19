namespace Infrastructure.ETLPipeline.Extract.Benifit
{
    public interface IBenefitRequest
    {
        public Task<List<BenefitResponse>> GetAllBenefitsAsync( string token );
    }
}
