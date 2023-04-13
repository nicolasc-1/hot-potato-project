namespace Ping.API.Communication;

public class Http : ICommunicationProvider
{
    private readonly string endpoint;

    public Http(string endpoint)
    {
        this.endpoint = endpoint;
    }

    public async Task<string> SendWithDelay(int hitsLeft, string route, int delay)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri($"http://{this.endpoint}")
        };

        await Task.Delay(delay);

        return await httpClient.GetStringAsync(@$"{route}?hitsLeft={hitsLeft}");
    }
}