using animeAlley.Data;
using animeAlley.Data.Seed;

namespace animeAlley.Data.Seed
{
    internal static class DbInitializerExtension
    {
        public static IApplicationBuilder UseItToSeedSqlServer(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                DbInitializer.Initialize(context).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
            }

            return app;
        }
    }
}