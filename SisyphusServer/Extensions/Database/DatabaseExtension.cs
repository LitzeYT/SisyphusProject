using SisyphusServer.Extensions.Database.Configs;
using SisyphusServer.Extensions.Database.Context;

namespace SisyphusServer.Extensions.Database {
    public static class DatabaseExtension {
        public static IServiceCollection AddDatabase<TIContext, TContext, TInitializer>(this IServiceCollection services, IConfiguration configuration)
            where TIContext : IBaseDatabaseContext
            where TContext : BaseDatabaseContext<TContext>, TIContext
            where TInitializer : class, IInitializer<TContext> {
            services.Configure<DatabaseConfig>(configuration.GetSection("Sql"));
            services.AddDbContext<TIContext, TContext>();
            services.AddScoped<IInitializer<TContext>, TInitializer>();
            return services;
        }

        public static IApplicationBuilder UseDatabase<TIContext, TContext>(this IApplicationBuilder app)
            where TIContext : IBaseDatabaseContext
            where TContext : BaseDatabaseContext<TContext>, TIContext {
            using (var scope = app.ApplicationServices.CreateScope()) {
                var context = (TContext) scope.ServiceProvider.GetRequiredService<TIContext>();
                var initializer = scope.ServiceProvider.GetRequiredService<IInitializer<TContext>>();
                initializer.Initialize(context, scope);
            }
            return app;
        }
    }
}
