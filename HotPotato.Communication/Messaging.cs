using HotPotato.Domain.Entities;
using HotPotato.Domain.Interfaces;

namespace HotPotato.Communication;

public class Messaging: ICommunicationProvider
{
    public async Task<string> Throw(string route, Potato potato)
    {
        throw new NotImplementedException();
    }
}