using Serilog;
using Serilog.Configuration;

namespace HotPotato.API;

public static class TracingEnricherExtension
{
    public static LoggerConfiguration WithTracingInformation(this LoggerEnrichmentConfiguration enrich)
    {
        if (enrich == null)
            throw new ArgumentNullException(nameof(enrich));

        return enrich.With<TracingEnricher>();
    }
}