using System.Diagnostics;
using HotPotato.Domain.Interfaces;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Serilog;

namespace HotPotato.Domain.Entities;

public class Potato
{
    private readonly ILogger logger = Log.Logger.ForContext<Potato>();
    private static readonly ActivitySource Source = new ActivitySource(nameof(Potato));
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    
    public int TimeToLive { get; set; } // number of throws before it gets dropped
    public int ThrowDelay { get; set; }
    public string Test { get; set; }

    public async Task<string> Throw(ICommunicationProvider communicationProvider, string instanceName)
    {
        Tick();
        this.logger.Information("Test for traces");
        
        Propagator.Inject(new PropagationContext(Activity.Current.Context, Baggage.Current), this, InjectContextIntoHeader);
        var parentContext = Propagator.Extract(default, this, ExtractFromPotato);
        using (var activity = Source.StartActivity("Throw a potato", ActivityKind.Internal, parentContext.ActivityContext))
        {
            return TimeToLive <= 0 ? 
                await Task.FromResult($"{instanceName} dropped the potato") :
                await WaitAndThrow(communicationProvider, nameof(Throw));
        }
    }
    
    private void Tick()
    {
        TimeToLive--;
    }
    
    private async Task<string> WaitAndThrow(ICommunicationProvider communicationProvider, string route)
    {
        await Task.Delay(ThrowDelay);
        return await communicationProvider.Throw(route, this);
    }
    
    private static IEnumerable<string> ExtractFromPotato(Potato arg1, string arg2)
    {
        return new[]{arg1.Test};
    }

    private static void InjectContextIntoHeader(Potato potato, string key, string value)
    {
        potato.Test = "";
    }
}