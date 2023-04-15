namespace Ping.API.Communication;

public class Http : ICommunicationProvider
{
    private readonly HttpClient httpClient;
    public Http(string endpoint)
    {
        this.httpClient = new HttpClient
        {
            BaseAddress = new Uri($"http://{endpoint}")
        };
    }

    public async Task<string> Send(int hitsLeft, string route, int thinkTime)
    {
        return await this.httpClient.GetStringAsync(@$"{route}?hitsLeft={hitsLeft}&thinkTime={thinkTime}");
    }
}