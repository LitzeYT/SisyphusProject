using Microsoft.AspNetCore.Mvc;

using Serilog;

using SisyphusServer.Extensions.Api;
using SisyphusServer.Extensions.Api.Filters;
using SisyphusServer.Extensions.Logger;
using SisyphusServer.Extensions.Swagger;
using SisyphusServer.Extensions.Version;

namespace SisyphusServer {
    public class ServiceBuilder {
        public string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        protected readonly WebApplicationBuilder Builder = WebApplication.CreateBuilder();
        protected WebApplication App;
        protected IConfiguration Configuration;

        private Action<WebApplication, IConfiguration> AddAppAction;

        public ServiceBuilder() {
            SetConfiguration();
            LogBuilder.CreateLogger(Configuration!);
            Log.Information(Builder.Environment.EnvironmentName);
        }

        public ServiceBuilder Build(List<Type> controllers) {
            Log.Information("Start building server...");

            Builder.Services.AddHttpContextAccessor();
            Builder.Services.AddApiExtension();

            Builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            var mvcBuilder = Builder.Services.AddControllers();
            mvcBuilder.AddMvcOptions(options => {
                options.Filters.Add<ApiExceptionFilterAttribute>();
            });
            foreach (var controller in controllers) {
                mvcBuilder.AddApplicationPart(controller.Assembly);
            }
            mvcBuilder.AddJsonOptions(option => option.JsonSerializerOptions.WriteIndented = true);

            Builder.Services.AddCustomSwagger();
            Builder.Services.AddCustomApiVersioning();

            App = Builder.Build();
            Log.Information("Application builded");
            
            return this;
        }

        public void Run() {
            Log.Information("Trying to run application...");
            if (App.Environment.IsDevelopment()) {
                App.UseDeveloperExceptionPage();
            } else {
                App.UseExceptionHandler("/Error");
                App.UseHsts();
            }

            App.UseStaticFiles();

            App.UseRouting();
            App.UseCustomSwagger();

            AddAppAction?.Invoke(App, Configuration);

            App.UseEndpoints(endpoint => {
                endpoint.MapControllers();
                endpoint.MapDefaultControllerRoute();
            });
            Log.Information("Starting web host...");
            App.Run();
        }

        public ServiceBuilder AddServices(Action<IServiceCollection, IConfiguration> action) {
            action(Builder.Services, Configuration);
            return this;
        }

        public ServiceBuilder AddApp(Action<WebApplication, IConfiguration> action) {
            AddAppAction = action;
            return this;
        }

        private void SetConfiguration() {
            var configBuilder = new ConfigurationBuilder();
            foreach (var file in Directory.GetFiles(BasePath)
                .Where(f => f.Contains("appsettings"))
                .Where(f => f.Contains(Builder.Environment.EnvironmentName))
                .Where(f => f.Contains(".json"))) {
                Builder.Configuration.AddJsonFile(file, optional: false, reloadOnChange: true);
                configBuilder.AddJsonFile(file, optional: false, reloadOnChange: true);
            }

            Builder.Configuration.AddEnvironmentVariables();
            configBuilder.AddEnvironmentVariables();

            Configuration = configBuilder.Build();
        }
    }
}
