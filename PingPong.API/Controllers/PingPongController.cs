using Microsoft.AspNetCore.Mvc;
using Ping.API.Communication;

namespace Ping.API.Controllers;

[ApiController]
public class PingPongController : ControllerBase
{
    private readonly ILogger<PingPongController> logger;
    private readonly ICommunicationProvider communicationProvider;

    public PingPongController(ILogger<PingPongController> logger)
    {
        this.logger = logger;
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
        return Enum.Parse<CommunicationMode>(Environment.GetEnvironmentVariable("MODE") ?? CommunicationMode.Http.ToString());
    }

    [HttpGet]
    [Route("Ping")]
    public async Task<string> Ping([FromQuery] int hitsLeft)
    {
        return hitsLeft <= 0 ? 
            await Task.FromResult("You lost!") : 
            await this.communicationProvider.SendWithDelay(--hitsLeft, nameof(Pong), 500);
    }
    
    [HttpGet]
    [Route("Pong")]
    public async Task<string> Pong([FromQuery] int hitsLeft)
    {
        return hitsLeft <= 0 ? 
            await Task.FromResult("You won!"):
            await this.communicationProvider.SendWithDelay(--hitsLeft, nameof(Ping), 500);
    }
}