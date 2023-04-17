using HotPotato.Domain.Entities;

namespace HotPotato.API.Communication;

public interface ICommunicationProvider
{
    public Task<string> Throw(string route, Potato potato);
}