using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Filters;

namespace SisyphusServer.Extensions.Swagger {
    public static class SwaggerExtension {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services) {
            services.AddSwaggerGen(options => {
                options.EnableAnnotations();
                options.ExampleFilters();
            });
            services.ConfigureOptions<ConfigureSwaggerOptions>();
            services.AddSwaggerExamplesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app) {
            app.UseSwagger();
            app.UseSwaggerUI(config => {
                var versionProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in versionProvider.ApiVersionDescriptions.Reverse()) {
                    config.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
            return app;
        }
    }
}
