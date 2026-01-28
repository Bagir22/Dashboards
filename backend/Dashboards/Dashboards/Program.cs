using Application;
using Hangfire;
using Infrastructure;
using Prometheus;

namespace WebApi
{
    public class Program
    {
        public static void Main( string[] args )
        {
            var builder = WebApplication.CreateBuilder( args );

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddHttpClient();
            builder.Services.AddWebApi( builder.Configuration );
            builder.Services.AddInfrastructure( builder.Configuration );
            builder.Services.AddApplication();

            var app = builder.Build();

            app.MigrateInfrastructure();

            app.MapHangfireDashboard();

            if ( app.Environment.IsDevelopment() )
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseHttpMetrics();

            app.MapControllers();

            app.Run();
        }
    }
}
