using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Persistence
{
    public static class DatabaseMigrator
    {
        public static void Migrate<T>( this IHost host ) where T : DbContext
        {
            using var scope = host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<T>();
            context.Database.Migrate();
        }
    }
}
