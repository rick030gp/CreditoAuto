using arquetipo.Infrastructure.Seeders;
using arquetipo.Repository.Context;
using System.Diagnostics.CodeAnalysis;

namespace arquetipo.API
{
    [ExcludeFromCodeCoverage]
    internal static class DbInitializerExtension
    {
        public static IApplicationBuilder UseItToSeedSqlServer(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<CrAutoDbContext>();
                SeederDb.Initialize(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return app;
        }
    }
}
