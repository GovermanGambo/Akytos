using Serilog;
using Serilog.Core;

namespace Akytos;

public static class Debug
{
    private static readonly Logger Logger;

    static Debug()
    {
        Logger = new LoggerConfiguration()
            .WriteTo.Console()
#if DEBUG || DEBUG_EDITOR
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .CreateLogger();
    }

    public static void LogInformation(string message)
    {
        Logger.Information(message);
    }
    
    public static void LogInformation<T0>(string message, T0 arg0)
    {
        Logger.Information(message, arg0);
    }
    
    public static void LogInformation<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        Logger.Information(message, arg0, arg1);
    }
    
    public static void LogInformation<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        Logger.Information(message, arg0, arg1, arg2);
    }
    
    public static void LogDebug(string message)
    {
        Logger.Debug(message);
    }
    
    public static void LogDebug<T0>(string message, T0 arg0)
    {
        Logger.Debug(message, arg0);
    }
    
    public static void LogDebug<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        Logger.Debug(message, arg0, arg1);
    }
    
    public static void LogDebug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        Logger.Debug(message, arg0, arg1, arg2);
    }
    
    public static void LogWarning(string message)
    {
        Logger.Warning(message);
    }
    
    public static void LogWarning<T0>(string message, T0 arg0)
    {
        Logger.Warning(message, arg0);
    }
    
    public static void LogWarning<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        Logger.Warning(message, arg0, arg1);
    }
    
    public static void LogWarning<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        Logger.Warning(message, arg0, arg1, arg2);
    }
    
    public static void LogError(string message)
    {
        Logger.Error(message);
    }
    
    public static void LogError<T0>(string message, T0 arg0)
    {
        Logger.Error(message, arg0);
    }
    
    public static void LogError<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        Logger.Error(message, arg0, arg1);
    }
    
    public static void LogError<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        Logger.Error(message, arg0, arg1, arg2);
    }
}