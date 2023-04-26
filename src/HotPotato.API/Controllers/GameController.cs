using HotPotato.API.Communication;
using HotPotato.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace HotPotato.API.Controllers;

[ApiController]
public class GameController : ControllerBase
{
    private readonly ICommunicationProvider communicationProvider;
    private readonly ILogger logger = Log.Logger.ForContext<GameController>();
    // private static readonly ActivitySource Source = new ActivitySource(nameof(GameController));
    private readonly string instanceName;

    public GameController(Instance instance)
    {
        this.instanceName = instance.Name;
        this.communicationProvider = 
            GetCommunicationMode() == CommunicationMode.Messaging ?
                new Messaging() : 
                new Http(GetEndpoint());
    }

    private static string GetEndpoint()
    {
        return Environment.GetEnvironmentVariable("ENDPOINT") ?? "localhost:5858";
    }

    private static CommunicationMode GetCommunicationMode()
    {
        return Enum.Parse<CommunicationMode>(Environment.GetEnvironmentVariable("MODE")
                                             ?? CommunicationMode.Http.ToString());
    }
    
    [HttpPost]
    [Route("Throw")]
    public async Task<string> Throw([FromBody] Potato potato)
    {
        potato.Tick();
        this.logger.ForContext<GameController>().Information("Test for traces");
        
        return potato.TimeToLive <= 0 ? 
            await Task.FromResult($"{this.instanceName} dropped the potato") :
            await WaitAndThrow(nameof(Throw), potato);
    }

    private async Task<string> WaitAndThrow(string route, Potato potato)
    {
        await Task.Delay(potato.ThrowDelay);
        return await this.communicationProvider.Throw(route, potato);
    }
}