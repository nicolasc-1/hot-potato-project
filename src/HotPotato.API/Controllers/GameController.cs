using HotPotato.Communication;
using HotPotato.Domain.Entities;
using HotPotato.Domain.Interfaces;
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
        return await potato.Throw(this.communicationProvider, this.instanceName);
    }
}