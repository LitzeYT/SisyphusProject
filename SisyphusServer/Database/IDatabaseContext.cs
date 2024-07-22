using Microsoft.EntityFrameworkCore;

using SisyphusServer.Database.Entities;
using SisyphusServer.Extensions.Database.Context;

namespace SisyphusServer.Database {
    public interface IDatabaseContext : IBaseDatabaseContext {
        DbSet<UserInfo> UserInfos { get; set; }
    }
}
