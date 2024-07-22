using FluentValidation;
using MediatR;
using Serilog;
using System.Diagnostics;

namespace SisyphusServer.Extensions.Api.Behaviours {
    public class InfrastructureBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> {
        private readonly IEnumerable<IValidator<TRequest>> Validators;

        public InfrastructureBehaviour(IEnumerable<IValidator<TRequest>> validators) {
            Validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken token) {
            var timer = new Stopwatch();
            timer.Start();
            var response = await HandleUnhandletExpection(request, next, token);
            timer.Stop();

            var time = timer.ElapsedMilliseconds;
            if (time > 500) {
                Log.Warning("Long running Request: {@Request}", new {
                    typeof(TResponse).Name,
                    Time = time,
                    Request = request
                });
            }
            return response;
        }

        private async Task<TResponse> HandleUnhandletExpection(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken token) {
            try {
                await HandleValidators(request, token);
                return await next();
            } catch (Exception e) {
                Log.Error(e, "Request: Unhandled Exception for Request {@Request}", new {
                    typeof(TRequest).Name,
                    e.Message,
                    Request = request
                });
                throw;
            }
        }

        private async Task HandleValidators(TRequest request, CancellationToken token) {
            if (Validators.Count() == 0) {
                return;
            }
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(Validators.Select(v => v.ValidateAsync(context, token)));
            var failures = validationResults
                .Where(r => r.Errors.Count != 0)
                .SelectMany(r => r.Errors)
                .ToList();
            if (failures.Count != 0) {
                Log.Error("Validation errors {@Data}", new {
                    typeof(TRequest).Name,
                    Request = request,
                    Failures = failures.Select(f => f.ErrorMessage)
                });
                throw new Exceptions.ValidationException(failures);
            }
        }
    }
}
