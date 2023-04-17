using HotPotato.API.Entities;

namespace HotPotato.API.Communication;

public class Messaging: ICommunicationProvider
{
    public async Task<string> Throw(string route, Potato potato)
    {
        throw new NotImplementedException();
    }
}