using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace Windmill.Services;

internal static class LoggerSinkConfigurationExtensions
{
    public static LoggerConfiguration ConsoleService(this LoggerSinkConfiguration loggerSinkConfiguration,
        ConsoleService consoleService, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose)
    {
        return loggerSinkConfiguration.Sink(new ConsoleServiceSink(consoleService), restrictedToMinimumLevel);
    }
}