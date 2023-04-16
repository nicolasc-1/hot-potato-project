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
    
    [HttpGet]
    [Route("Throw")]
    public async Task<string> Throw([FromQuery] int throwsLeft,[FromQuery] int thinkTime)
    {
        return throwsLeft <= 0 ? 
            await Task.FromResult($"{this.instanceName} dropped the potato") :
            await WaitAndSend(--throwsLeft, nameof(Throw), thinkTime);
    }

    private async Task<string> WaitAndSend(int throwsLeft, string route, int thinkTime)
    {
        await Task.Delay(thinkTime);
        return await this.communicationProvider.Send(throwsLeft, route, thinkTime);
    }
}