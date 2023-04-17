using HotPotato.API.Communication;
using HotPotato.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HotPotato.API.Controllers;

[ApiController]
public class GameController : ControllerBase
{
    private readonly ICommunicationProvider communicationProvider;
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