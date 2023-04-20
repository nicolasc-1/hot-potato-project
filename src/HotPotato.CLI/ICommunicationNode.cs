namespace HotPotato.CLI;

public interface ICommunicationNode
{
    public string Endpoint { get; }
    public string ToCompose();
}