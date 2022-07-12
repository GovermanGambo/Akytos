using System;
using Akytos.Diagnostics.Logging;
using Serilog.Core;

namespace Windmill.Diagnostics.Logging;

internal class ConsoleServiceSink : ILogEventSink
{
    private readonly ConsoleService m_consoleService;

    public ConsoleServiceSink(ConsoleService consoleService)
    {
        m_consoleService = consoleService;
    }

    public void Emit(Serilog.Events.LogEvent logEvent)
    {
        var logLevel = Enum.Parse<LogLevel>(logEvent.Level.ToString());
        string message = logEvent.RenderMessage();

        LogEvent akytosLogEvent;

        if (logEvent.Exception != null)
        {
            akytosLogEvent = new LogEvent(logEvent.Exception, logLevel);
        }
        else
        {
            akytosLogEvent = new LogEvent(logLevel.ToString(), message, logLevel);
        }
        
        m_consoleService.RegisterLogEvent(akytosLogEvent);
    }
}