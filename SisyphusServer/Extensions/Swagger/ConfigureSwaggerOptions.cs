using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Serilog;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace SisyphusServer.Extensions.Swagger {
    public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions> {
        private readonly IApiVersionDescriptionProvider Provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) {
            Provider = provider;
        }

        public void Configure(SwaggerGenOptions options) {
            foreach (var item in Provider.ApiVersionDescriptions) {
                options.SwaggerDoc(item.GroupName, CreateVersionInfo(item));
            }
        }

        public void Configure(string? name, SwaggerGenOptions options) {
            Configure(options);
        }

        private OpenApiInfo CreateVersionInfo(ApiVersionDescription item) {
            var info = new OpenApiInfo() {
                Title = item.GroupName,
                Version = item.ApiVersion.ToString()
            };
            if (item.IsDeprecated) {
                info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";
            }
            Log.Debug("Api version added {@Item}", info);
            return info;
        }
    }
}
