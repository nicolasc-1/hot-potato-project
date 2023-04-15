using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Prometheus;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog((_, _, configuration) =>
{
    configuration
        .WriteTo.File(
            path: "/logs/apps/pingpong.jsonl",
            flushToDiskInterval: TimeSpan.FromSeconds(10),
            rollingInterval: RollingInterval.Day,
            retainedFileTimeLimit: TimeSpan.FromDays(5),
            retainedFileCountLimit: 5,
            rollOnFileSizeLimit: false,
            formatter: new RenderedCompactJsonFormatter())
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Error)
        .MinimumLevel.Override("Default", LogEventLevel.Information)
        .Enrich.WithProperty("instance_name", Faker.NameFaker.FirstName())
        .Filter.ByExcluding(
            Matching.WithProperty<string>("RequestPath", v =>
                "/metrics".Equals(v, StringComparison.OrdinalIgnoreCase) ||
                "/favicon.ico".Equals(v, StringComparison.OrdinalIgnoreCase)))
        .WriteTo.Console();
});

// Add services to the container.
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseAuthorization();
app.UseHttpMetrics();

app.MapControllers();
app.MapMetrics();
Metrics.DefaultRegistry.SetStaticLabels(new Dictionary<string, string>
{
    { "appname", "pingpong" }
});

try
{
    app.Start();
    Log.Information("App started, listening on {Endpoint}", string.Join(";",
            app.Services.GetService<IServer>()?.Features.Get<IServerAddressesFeature>()?.Addresses ??
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