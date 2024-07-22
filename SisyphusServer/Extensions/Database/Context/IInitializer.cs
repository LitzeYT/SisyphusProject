using Microsoft.EntityFrameworkCore;

namespace SisyphusServer.Extensions.Database.Context {
    public interface IInitializer<T> where T : DbContext {
        void Initialize(BaseDatabaseContext<T> context, IServiceScope scope);
    }
}
