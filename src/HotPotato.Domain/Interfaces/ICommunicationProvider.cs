using HotPotato.Domain.Entities;

namespace HotPotato.Domain.Interfaces;

public interface ICommunicationProvider
{
    public Task<string> Throw(string route, Potato potato);
}