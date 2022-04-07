using Serilog;
using Serilog.Core;

namespace Akytos.Diagnostics.Logging;

internal static class Log
{
    private static Logger? s_clientLogger;
    private static Logger? s_coreLogger;
    
    public static void InitializeLogging(Action<LoggerConfiguration, LoggerConfiguration> configureLogs)
    {
        var coreConfiguration = new LoggerConfiguration();
        var clientConfiguration = new LoggerConfiguration();
        configureLogs(coreConfiguration, clientConfiguration);

        s_coreLogger = coreConfiguration.CreateLogger();
        s_clientLogger = clientConfiguration.CreateLogger();
    }

    public static Logger Client
    {
        get
        {
            if (s_clientLogger is null)
            {
                throw new Exception("Logging has not been initialized!");
            }

            return s_clientLogger;
        }
    }

    public static Logger Core
    {
        get
        {
            if (s_coreLogger is null)
            {
                throw new Exception("Logging has not been initialized!");
            }

            return s_coreLogger;
        }
    }
}