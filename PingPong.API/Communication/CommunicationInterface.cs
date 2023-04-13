namespace Ping.API.Communication;

public interface ICommunicationProvider
{
    public Task<string> SendWithDelay(int hitsLeft, string route, int delay);
}