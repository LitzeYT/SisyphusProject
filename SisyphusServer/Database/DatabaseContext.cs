using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using SisyphusServer.Database.Entities;
using SisyphusServer.Extensions.Database.Configs;
using SisyphusServer.Extensions.Database.Context;

namespace SisyphusServer.Database {
    public class DatabaseContext : BaseDatabaseContext<DatabaseContext>, IDatabaseContext {
        public DbSet<UserInfo> UserInfos { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options, IOptions<DatabaseConfig> config) : base(options, config) { }
    }
}
