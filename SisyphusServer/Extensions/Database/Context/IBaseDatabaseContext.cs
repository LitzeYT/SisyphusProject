using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SisyphusServer.Extensions.Database.Context {
    public interface IBaseDatabaseContext {
        DatabaseFacade Database { get; }
        ChangeTracker ChangeTracker { get; }

        Task<int> ExecuteSqlRawAsync(string sql, params object[] parameter);
        Task<int> SaveChangesAsync(CancellationToken token);
    }
}
