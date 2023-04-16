namespace HotPotato.API.Communication;

public interface ICommunicationProvider
{
    public Task<string> Send(int length, string route, int thinkTime);
}