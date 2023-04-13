using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseSerilog((_, _, configuration) => {
    configuration
        .WriteTo.File(
            path: "pingpong.jsonl",
            flushToDiskInterval: TimeSpan.FromSeconds(10),
            rollingInterval: RollingInterval.Day,
            retainedFileTimeLimit: TimeSpan.FromDays(5),
            retainedFileCountLimit: 5,
            rollOnFileSizeLimit: false,
            formatter: new RenderedCompactJsonFormatter())
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Error)
        .MinimumLevel.Override("Default", LogEventLevel.Information)
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

app.MapControllers();

try
{
    app.Run();
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
