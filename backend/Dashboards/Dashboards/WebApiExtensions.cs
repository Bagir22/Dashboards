using Microsoft.OpenApi.Models;
using System.Reflection;

namespace WebApi
{
    public static class WebApiExtensions
    {
        public static IServiceCollection AddWebApi( this IServiceCollection services, IConfiguration configuration )
        {
            AddSwagger( services );

            return services;
        }

        private static void AddSwagger( IServiceCollection services )
        {
            services.AddSwaggerGen( c =>
            {
                c.SwaggerDoc( "v1", new OpenApiInfo
                {
                    Title = "UniDash",
                    Version = "v1",
                    Description = "API для отслеживания статистики контингента студентов",
                    Contact = new OpenApiContact { Name = "Разработчики Йошкар-Олы", Email = "dev@example.com" }
                } );

                // Включение XML-комментариев
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine( AppContext.BaseDirectory, xmlFile );
                c.IncludeXmlComments( xmlPath );
            } );
        }
    }
}
