using Serilog;
using Serilog.Configuration;

namespace HotPotato.Telemetry;

public static class TracingEnricherExtension
{
    public static LoggerConfiguration WithTracingInformation(this LoggerEnrichmentConfiguration enrich)
    {
        if (enrich == null)
            throw new ArgumentNullException(nameof(enrich));

        return enrich.With<TracingEnricher>();
    }
}