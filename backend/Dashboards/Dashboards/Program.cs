using Application;
using DotNetEnv;
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

            Env.Load("../../../.env");
            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddHttpClient();
            builder.Services.AddWebApi( builder.Configuration );
            builder.Services.AddInfrastructure( builder.Configuration );
            builder.Services.AddApplication();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    var host = Environment.GetEnvironmentVariable("BASE_URL");
                    var port = Environment.GetEnvironmentVariable("HOST_PORT");
                    policy.WithOrigins($"http://{host}:{port}")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            app.MigrateInfrastructure();

            app.MapHangfireDashboard();

            if ( app.Environment.IsDevelopment() )
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseCors("AllowAngularApp");

            app.UseAuthorization();

            app.UseHttpMetrics();

            app.MapControllers();

            app.Run();
        }
    }
}
