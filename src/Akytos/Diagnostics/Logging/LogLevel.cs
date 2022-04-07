namespace Akytos.Diagnostics.Logging;

[Flags]
public enum LogLevel
{
    Debug,
    Information,
    Warning,
    Error,
    Fatal,
    All = Debug | Information | Warning | Error | Fatal
}