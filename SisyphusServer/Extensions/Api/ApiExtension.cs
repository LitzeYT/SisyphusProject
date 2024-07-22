using FluentValidation;
using FluentValidation.AspNetCore;

using MediatR.Pipeline;
using MediatR;
using SisyphusServer.Extensions.Api.Behaviours;

namespace SisyphusServer.Extensions.Api {
    public static class ApiExtension {
        public static IServiceCollection AddApiExtension(this IServiceCollection services) {
            services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            services.AddFluentValidationClientsideAdapters();
            services.AddMediatR(config => {
                config.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

                config.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>));
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(InfrastructureBehaviour<,>));
            });
            return services;
        }
    }
}
