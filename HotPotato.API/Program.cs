using HotPotato.API;
using HotPotato.Domain.Entities;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var instance = new Instance();
SerilogConfigurator.Build(builder, instance.Name);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton(instance);

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
    { "appname", "hotpotato" }
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