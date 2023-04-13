namespace Ping.API.Communication;

public class Messaging: ICommunicationProvider
{
    public Task<string> SendWithDelay(int hitsLeft, string route, int delay)
    {
        throw new NotImplementedException();
    }
}