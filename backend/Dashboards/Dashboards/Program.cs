using Application;
using Hangfire;
using Infrastructure;
using Prometheus;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавляем явное логирование в консоль
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddHttpClient();
            builder.Services.AddWebApi(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            // CORS настройки
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            builder.Logging.AddConsole().AddFilter("Microsoft", LogLevel.Warning);
            var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger("Program");

            if (allowedOrigins != null)
            {
                logger.LogInformation("Разрешенные CORS источники: {Origins}", string.Join(", ", allowedOrigins));
            }
            else
            {
                logger.LogWarning("CORS источники не настроены в appsettings.json");
            }

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins(allowedOrigins ?? new[] { "http://localhost:4200" })
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();

                        logger.LogInformation("CORS политика применена");
                    });
            });

            var app = builder.Build();

            // Логируем каждый входящий запрос (для отладки CORS)
            app.Use(async (context, next) =>
            {
                var requestLogger = app.Logger;
                requestLogger.LogInformation(
                    "Входящий запрос: {Method} {Path} от {Origin}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.Headers["Origin"].ToString());

                await next();
            });

            app.MigrateInfrastructure();
            app.MapHangfireDashboard();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseAuthorization();
            app.UseHttpMetrics();
            app.MapControllers();

            app.Logger.LogInformation("Приложение запущено. Порт: 8080");
            app.Run();
        }
    }
}
