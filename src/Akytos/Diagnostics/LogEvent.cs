namespace Akytos.Diagnostics;

internal class LogEvent
{
    public LogEvent(string header, string message, LogLevel logLevel)
    {
        Header = header;
        Message = message;
        LogLevel = logLevel;
    }

    public LogEvent(Exception exception, LogLevel logLevel)
    {
        Header = exception.GetType().Name;
        Message = exception.StackTrace ?? exception.Message;
        LogLevel = logLevel;
    }

    public string Header { get; }
    public string Message { get; }
    public LogLevel LogLevel { get; }
    public Exception? Exception { get; }
}