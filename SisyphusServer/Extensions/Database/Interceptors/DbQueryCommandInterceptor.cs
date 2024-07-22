using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using System.Data.Common;

namespace SisyphusServer.Extensions.Database.Interceptors {
    public class DbQueryCommandInterceptor : DbCommandInterceptor {
        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result) {
            LogTheCommandQuery(command);
            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken token) {
            LogTheCommandQuery(command);
            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }

        private void LogTheCommandQuery(DbCommand command) {
            var query = command.CommandText;
            foreach (DbParameter parameter in command.Parameters) {
                query = query.Replace(parameter.ParameterName, parameter.Value?.ToString());
            }
            Log.Information("Query intercepted {@Data}", new {
                Query = query
            });
        }
    }
}
