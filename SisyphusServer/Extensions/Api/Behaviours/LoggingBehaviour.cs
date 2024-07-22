using MediatR.Pipeline;

using Serilog;

namespace SisyphusServer.Extensions.Api.Behaviours {
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull {
        public async Task Process(TRequest request, CancellationToken token) {
            await Task.Run(() => Log.Information("Request: {@Request}", new {
                typeof(TRequest).Name,
                Request = request
            }), token);
        }
    }
}
