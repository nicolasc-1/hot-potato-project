using Microsoft.AspNetCore.Mvc;
using Ping.API.Communication;

namespace Ping.API.Controllers;

[ApiController]
public class PingPongController : ControllerBase
{
    private readonly ICommunicationProvider communicationProvider;

    public PingPongController()
    {
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
    [Route("Play")]
    public async Task<string> Play([FromQuery] int numberOfExchanges,[FromQuery] int thinkTime)
    {
        return await this.communicationProvider.Send(numberOfExchanges, nameof(PingPong));
    }
    
    [HttpGet]
    [Route("PingPong")]
    public async Task<string> PingPong([FromQuery] int hitsLeft,[FromQuery] int thinkTime)
    {
        return hitsLeft <= 0 ? 
            await Task.FromResult(new Random().Next(0, 100) > 50 ? "You won!" : "You lost...") :
            await WaitAndSend(--hitsLeft, nameof(PingPong), thinkTime);
    }

    private async Task<string> WaitAndSend(int hitsLeft, string route, int thinkTime)
    {
        await Task.Delay(thinkTime);
        return await this.communicationProvider.Send(hitsLeft, route);
    }
}