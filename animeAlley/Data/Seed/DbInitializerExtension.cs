using animeAlley.Data;
using AppFotos.Data.Seed;

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
                DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                // Log the error (uncomment ex variable name and write a log)
            }

            return app;
        }
    }

}