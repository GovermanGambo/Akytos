using System.Collections.Generic;
using System.Linq;
using Akytos.Diagnostics.Logging;

namespace Windmill.Diagnostics.Logging;

internal class ConsoleService
{
    private const int MaxPageSize = 256;

    private readonly List<LogEvent> m_logEvents;
    private List<LogEvent> m_filteredLogs;

    public ConsoleService()
    {
        m_logEvents = new List<LogEvent>();
        m_filteredLogs = m_logEvents;
    }

    public void ApplyFilter(LogLevel logLevel)
    {
        m_filteredLogs = m_logEvents.Where(l => (l.LogLevel | logLevel) != 0).ToList();
    }
    
    public void RegisterLogEvent(LogEvent logEvent)
    {
        m_logEvents.Add(logEvent);
    }
    
    public IEnumerable<LogEvent> GetLogs(int offset = 0, int pageSize = MaxPageSize)
    {
        return m_filteredLogs.GetRange(offset, pageSize);
    }
}