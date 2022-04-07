using Log = Akytos.Diagnostics.Logging.Log;

namespace Akytos;

/// <summary>
///     A static class used for logging debugging information to the console.
/// </summary>
public static class Debug
{
    public static void LogVerbose(string message)
    {
        Log.Core.Verbose(message);
    }

    public static void LogVerbose<T0>(string message, T0 arg0)
    {
        Log.Client.Verbose(message, arg0);
    }

    public static void LogVerbose<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        Log.Client.Verbose(message, arg0, arg1);
    }

    public static void LogVerbose<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        Log.Client.Verbose(message, arg0, arg1, arg2);
    }
    
    public static void LogInformation(string message)
    {
        Log.Client.Information(message);
    }
    
    public static void LogInformation<T0>(string message, T0 arg0)
    {
        Log.Client.Information(message, arg0);
    }
    
    public static void LogInformation<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        Log.Client.Information(message, arg0, arg1);
    }
    
    public static void LogInformation<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        Log.Client.Information(message, arg0, arg1, arg2);
    }
    
    public static void LogDebug(string message)
    {
        Log.Client.Debug(message);
    }
    
    public static void LogDebug<T0>(string message, T0 arg0)
    {
        Log.Client.Debug(message, arg0);
    }
    
    public static void LogDebug<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        Log.Client.Debug(message, arg0, arg1);
    }
    
    public static void LogDebug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        Log.Client.Debug(message, arg0, arg1, arg2);
    }
    
    public static void LogWarning(string message)
    {
        Log.Client.Warning(message);
    }
    
    public static void LogWarning<T0>(string message, T0 arg0)
    {
        Log.Client.Warning(message, arg0);
    }
    
    public static void LogWarning<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        Log.Client.Warning(message, arg0, arg1);
    }
    
    public static void LogWarning<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        Log.Client.Warning(message, arg0, arg1, arg2);
    }

    public static void LogError(Exception exception, string message)
    {
        Log.Client.Error(exception, message);
    }
    
    public static void LogError<T0>(Exception exception, string message, T0 arg0)
    {
        Log.Client.Error(exception, message, arg0);
    }
    
    public static void LogError<T0, T1>(Exception exception, string message, T0 arg0, T1 arg1)
    {
        Log.Client.Error(exception, message, arg0, arg1);
    }
    
    public static void LogError<T0, T1, T2>(Exception exception, string message, T0 arg0, T1 arg1, T2 arg2)
    {
        Log.Client.Error(exception, message, arg0, arg1, arg2);
    }

    public static void LogError(string message)
    {
        Log.Client.Error(message);
    }
    
    public static void LogError<T0>(string message, T0 arg0)
    {
        Log.Client.Error(message, arg0);
    }
    
    public static void LogError<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        Log.Client.Error(message, arg0, arg1);
    }
    
    public static void LogError<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        Log.Client.Error(message, arg0, arg1, arg2);
    }
}