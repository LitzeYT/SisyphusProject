using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

using SisyphusServer.Extensions.Logger.Configs;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SisyphusServer.Extensions.Logger {
    public static class LogBuilder {
        private const string DEFAULT_OUTPUT_TEMPLATE = "[{AppName} {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {StackTrace} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        public static void CreateLogger(IConfiguration configuration) {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
                var message = ((Exception) e.ExceptionObject).Message;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    EventLog.WriteEntry("SisyphusServer", message, EventLogEntryType.Error);
                }
                Console.WriteLine("SisyphusServer: " + message);
            };

            var logConfig = configuration.GetSection("Log").Get<LogConfig>();
            var loggerConfiguration = new LoggerConfiguration();

            switch (logConfig.MinimumLevel.ToUpper()) {
                case "DEBUG":
                    loggerConfiguration.MinimumLevel.Debug();
                    break;
                case "INFO":
                case "INFORMATION":
                    loggerConfiguration.MinimumLevel.Information();
                    break;
                case "WARN":
                case "WARNING":
                    loggerConfiguration.MinimumLevel.Warning();
                    break;
                case "ERROR":
                    loggerConfiguration.MinimumLevel.Error();
                    break;
                case "FATAL":
                    loggerConfiguration.MinimumLevel.Fatal();
                    break;
                default:
                    loggerConfiguration.MinimumLevel.Verbose();
                    break;
            }

            loggerConfiguration.WriteTo.Console(
                outputTemplate: logConfig.MessageTemplate ?? DEFAULT_OUTPUT_TEMPLATE,
                theme: AnsiConsoleTheme.Literate
            );

            if (!string.IsNullOrWhiteSpace(logConfig.FilePath)) {
                loggerConfiguration.WriteTo.File(
                    path: logConfig.FilePath,
                    outputTemplate: logConfig.MessageTemplate ?? DEFAULT_OUTPUT_TEMPLATE,
                    rollingInterval: RollingInterval.Month
                );
            }

            loggerConfiguration.Enrich.FromLogContext();
            loggerConfiguration.Enrich.WithProperty("AppName", "SisyphusServer");

            Log.Logger = loggerConfiguration.CreateLogger();
            Log.Information("Logger configurated");
        }
    }
}
