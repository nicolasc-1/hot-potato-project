using System.Diagnostics;
using System.Reflection;
using HotPotato.API;
using HotPotato.Domain.Entities;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var instance = Instance.FromEnvironmentVariable();
SerilogBuilder.Build(builder, instance.Name);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton(instance);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Telemetry
builder.Services
    .AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
        
        tracing.AddAspNetCoreInstrumentation(options =>
        {
            options.RecordException = true;
            options.EnrichWithHttpRequest = ActivityEnricher.EnrichHttpRequest;
            options.EnrichWithHttpResponse = ActivityEnricher.EnrichHttpResponse;
            options.Filter = context => context.Request.Path == "/Throw";
        });

        var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()!.Version;
        tracing.SetResourceBuilder(ResourceBuilder
            .CreateEmpty()
            .AddService(builder.Environment.ApplicationName, version)
            .AddAttributes(
                new KeyValuePair<string, object>[]
                {
                    new("host.name",  instance.Name)
                })
            .AddEnvironmentVariableDetector());

        // in debug, activate the console output
        if (builder.Environment.IsDevelopment())
        {
            tracing.AddConsoleExporter();
        }
        
        tracing.AddOtlpExporter(options =>
        {
            var host = builder.Environment.IsDevelopment() ? "localhost" : "tempo";
            options.Endpoint = new Uri($"http://{host}:4317");
            options.Protocol = OtlpExportProtocol.Grpc;
        });

        // Listen to telemetry coming from GameController
        // tracing.AddSource(nameof(GameController));
        // var listener = new ActivityListener
        // {
        //     ShouldListenTo = source => source.Name == nameof(GameController),
        //     SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData,
        //     Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData 
        // };
        // ActivitySource.AddActivityListener(listener);
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.UseAuthorization();
app.UseHttpMetrics();

app.MapControllers();
app.MapMetrics();
Metrics.DefaultRegistry.SetStaticLabels(new Dictionary<string, string>
{
    { "appname", "hotpotato" }
});

try
{
    app.Start();
    Log.Information("{InstanceName} started, listening on {Endpoint}", 
        instance.Name, 
        string.Join(";", app.Services.GetService<IServer>()?.Features.Get<IServerAddressesFeature>()?.Addresses ??
                         throw new InvalidOperationException("Problem getting server information")));
    app.WaitForShutdown();
}
catch (Exception e)
{
    Log.Fatal(e, "A fatal error occured, couldn't recover. Trying to flush log buffer...");
}
finally
{
    Log.CloseAndFlush();
    Environment.Exit(-1);
}