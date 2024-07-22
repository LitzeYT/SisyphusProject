using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SisyphusServer.Extensions.Database.Configs;
using SisyphusServer.Extensions.Database.Interceptors;

namespace SisyphusServer.Extensions.Database.Context {
    public class BaseDatabaseContext<T> : DbContext, IBaseDatabaseContext, IDisposable where T : DbContext {
        private readonly DatabaseConfig DatabaseConfig;

        public BaseDatabaseContext(DbContextOptions<T> dbContextOptions, IOptions<DatabaseConfig> databaseConfig) : base(dbContextOptions) {
            DatabaseConfig = databaseConfig.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer($"Server={DatabaseConfig.Server};Database={DatabaseConfig.Database};User Id={DatabaseConfig.Username};Password={DatabaseConfig.Password};MultipleActiveResultSets=True;Encrypt=false;TrustServerCertificate=True;");
            optionsBuilder.AddInterceptors(new DbQueryCommandInterceptor());
        }

        public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters) {
            return await Database.ExecuteSqlRawAsync(sql, parameters);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
