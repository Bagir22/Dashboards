using System.Reflection;
using Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication( this IServiceCollection services )
        {
            services.AddMediatR( cfg =>
            cfg.RegisterServicesFromAssembly( Assembly.GetExecutingAssembly() ) );

            services.AddAutoMapper( cfg => cfg.AddProfile<FilterProfile>() );

            return services;
        }
    }
}
