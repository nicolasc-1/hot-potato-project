namespace HotPotato.CLI.Entities;

public interface ICommunicationNode
{
    public string Endpoint { get; }
    public string ToCompose();
}