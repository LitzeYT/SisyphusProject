using Microsoft.EntityFrameworkCore;
using Serilog;
using SisyphusServer.Extensions.Database.Exceptions;

namespace SisyphusServer.Extensions.Database.Context {
    public class Initializer<T> : IInitializer<T> where T : DbContext {
        public void Initialize(BaseDatabaseContext<T> context, IServiceScope scope) {
            try {
                var success = context.Database.CanConnect();
                Log.Information("Initialize.Success: " + (success ? "Ja" : "Nein") + "; " + context.Database.ProviderName);
                if (!success) {
                    throw new DatabaseException("Can not connect to database!");
                }
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            } catch (Exception ex) {
                Log.Error(ex, "Initialize database failed.");
            }
        }
    }
}
