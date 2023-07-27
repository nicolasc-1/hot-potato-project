using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace HotPotato.Telemetry;

public class TracingEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (Activity.Current == null) return;
        
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", Activity.Current.Context.TraceId));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", Activity.Current.Context.SpanId));
    }
}