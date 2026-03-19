namespace Infrastructure.ETLPipeline.Extract.Validation
{
    internal static class RequestValidation
    {
        public static void ValidateResponse( HttpResponseMessage response )
        {
            if ( !response.IsSuccessStatusCode )
                throw new HttpRequestException( $"Status code: {response.StatusCode}" );
        }
    }
}
