using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ETLPipeline
{
    public class Worker( IServiceScopeFactory serviceScopeFactory, ILogger<Worker> logger ): IHostedService
    {
        public async Task StartAsync( CancellationToken cancellationToken )
        {
            using var scope = serviceScopeFactory.CreateScope();

            var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
            var dataSynchronizer = scope.ServiceProvider.GetRequiredService<IDataSynchronizer>();
            var backgroundJobClient = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();

            //recurringJobManager.AddOrUpdate(
            //    "add-data-monthly",
            //    () => dataSynchronizer.UpdateData(),
            //    "0 0 1 * *",
            //    new RecurringJobOptions
            //    {
            //        TimeZone = TimeZoneInfo.Local,
            //        MisfireHandling = MisfireHandlingMode.Relaxed
            //    });

            logger.LogInformation( "Recurring job 'add-data-monthly' registered with Hourly schedule" );

            backgroundJobClient.Enqueue( () => dataSynchronizer.InitialCreate() );
            logger.LogInformation( "Initial data sync job enqueued" );
        }

        public Task StopAsync( CancellationToken cancellationToken )
        {
            logger.LogInformation( "Worker service stopping" );
            return Task.CompletedTask;
        }
    }
}
