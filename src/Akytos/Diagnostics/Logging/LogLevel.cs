namespace Akytos.Diagnostics.Logging;

[Flags]
public enum LogLevel
{
    Verbose = 1,
    Debug = 2,
    Information = 4,
    Warning = 8,
    Error = 16,
    Fatal = 32,
    All = Debug | Information | Warning | Error | Fatal
}