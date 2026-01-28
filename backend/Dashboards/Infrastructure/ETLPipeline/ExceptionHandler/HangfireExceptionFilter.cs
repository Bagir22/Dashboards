using Hangfire.Common;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ETLPipeline.ExceptionHandler
{
    public class HangfireExceptionFilter: JobFilterAttribute, IServerFilter
    {
        private readonly ILogger<HangfireExceptionFilter> _logger;
        private readonly IReadOnlyDictionary<Type, Action<Exception>> _exceptionHandlers;

        public HangfireExceptionFilter( ILogger<HangfireExceptionFilter> logger )
        {
            _logger = logger;
            _exceptionHandlers = new Dictionary<Type, Action<Exception>>
            {
                [ typeof( HttpRequestException ) ] = ex => LogException( ex, "HttpRequestException" ),
                [ typeof( FileNotFoundException ) ] = ex => LogException( ex, "FileNotFoundException" ),
                [ typeof( InvalidOperationException ) ] = ex => LogException( ex, "InvalidOperationException" ),
                [ typeof( IOException ) ] = ex => LogException( ex, "IOException" )
            };
        }

        public void OnPerforming( PerformingContext context )
        {
        }

        public void OnPerformed( PerformedContext context )
        {
            var exception = context.Exception;
            if ( exception == null )
                return;

            var actualException = exception is JobPerformanceException
               ? exception.InnerException ?? exception
               : exception;

            foreach ( var handler in _exceptionHandlers )
            {
                if ( handler.Key.IsInstanceOfType( actualException ) )
                {
                    handler.Value( actualException );
                    return;
                }
            }

            LogException( exception, "Unexpected error" );
        }


        private void LogException( Exception ex, string errorType )
        {
            _logger.LogError( ex, "[{DateTime}] {ErrorType} \nStackTrace: {Message}",
                DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ), errorType, ex.Message );
        }
    }
}
