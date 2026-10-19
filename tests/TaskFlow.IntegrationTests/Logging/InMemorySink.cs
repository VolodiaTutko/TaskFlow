using System.Collections.Concurrent;
using Serilog.Core;
using Serilog.Events;

namespace TaskFlow.IntegrationTests.Logging;

public sealed class InMemorySink : ILogEventSink
{
    public ConcurrentQueue<LogEvent> Events { get; } = new();

    public void Emit(LogEvent logEvent)
    {
        Events.Enqueue(logEvent);
    }
}
