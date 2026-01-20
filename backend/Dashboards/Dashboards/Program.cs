namespace Dashboards;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration
            .AddEnvironmentVariables();
        
        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}