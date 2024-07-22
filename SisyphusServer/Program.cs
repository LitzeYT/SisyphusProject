using Serilog;

using SisyphusServer;
using SisyphusServer.Controllers;
using SisyphusServer.Database;
using SisyphusServer.Extensions.Database;
using SisyphusServer.Extensions.Database.Context;

try {
    new ServiceBuilder().AddServices((services, configuration) => {
        services.AddDatabase<IDatabaseContext, DatabaseContext, Initializer<DatabaseContext>>(configuration);
    }).Build([typeof(UserController)])
    .AddApp((app, configuration) => {
        app.UseDatabase<IDatabaseContext, DatabaseContext>();
    }).Run();
} catch (Exception ex) {
    Console.WriteLine(ex);
} finally {
    Log.CloseAndFlush();
}