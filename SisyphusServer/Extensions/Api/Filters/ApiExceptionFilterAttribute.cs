using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Serilog;

using SisyphusServer.Extensions.Api.Exceptions;

namespace SisyphusServer.Extensions.Api.Filters {
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute {
        private readonly IDictionary<Type, Action<ExceptionContext>> ExceptionHandlers;

        public ApiExceptionFilterAttribute() {
            ExceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>() {
                { typeof(ValidationException), HandleValidationException }
            };
        }

        public override void OnException(ExceptionContext context) {
            HandleException(context);
            base.OnException(context);
        }

        private void HandleException(ExceptionContext context) {
            Log.Error(context.Exception.Message);
            Type type = context.Exception.GetType();
            if (ExceptionHandlers.ContainsKey(type)) {
                ExceptionHandlers[type].Invoke(context);
                return;
            }
            if (!context.ModelState.IsValid) {
                HandleInvalidModelStateException(context);
                return;
            }
            HandleUnknownException(context);
        }

        private void HandleValidationException(ExceptionContext context) {
            var exception = (ValidationException) context.Exception;
            var details = new ValidationProblemDetails(exception.Errors) {
                Type = "dp.error.invalidInput"
            };
            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }

        private static void HandleInvalidModelStateException(ExceptionContext context) {
            var details = new ValidationProblemDetails(context.ModelState) {
                Type = "dp.error.invalidInput"
            };
            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }

        private void HandleUnknownException(ExceptionContext context) {
            var details = new ProblemDetails {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "dp.error.unknownException"
            };
            context.Result = new ObjectResult(details) {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
            Log.Error("UnknownException: {@Data}", new {
                context.Exception.Message,
                InnerMessage = context.Exception?.InnerException?.Message ?? "",
                context.Exception.StackTrace
            });
        }
    }
}
