﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Compact;

namespace HotPotato.Telemetry;

public static class SerilogBuilder
{
    public static void Build(WebApplicationBuilder builder, string instanceName)
    {
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((_, _, configuration) =>
        {
            configuration
                .WriteTo.File(
                    path: $"/logs/apps/{instanceName}.jsonl",
                    flushToDiskInterval: TimeSpan.FromSeconds(10),
                    rollingInterval: RollingInterval.Day,
                    retainedFileTimeLimit: TimeSpan.FromDays(1),
                    retainedFileCountLimit: 1,
                    rollOnFileSizeLimit: false,
                    formatter: new RenderedCompactJsonFormatter())
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .MinimumLevel.Override("Default", LogEventLevel.Information)
                .Enrich.WithProperty("instance_name", instanceName)
                .Enrich.WithTracingInformation()
                .Filter.ByExcluding(
                    Matching.WithProperty<string>("RequestPath", v =>
                        "/metrics".Equals(v, StringComparison.OrdinalIgnoreCase) ||
                        "/favicon.ico".Equals(v, StringComparison.OrdinalIgnoreCase)))
                .WriteTo.Console();
        });
    }
}