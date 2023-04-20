namespace HotPotato.CLI.Entities;

public static class PortMapping
{
    public static List<string> Get(int startingPort, int count)
    {
        List<string> portList = new List<string>();
        for (int i = startingPort; i < startingPort + count; i++)
        {
            portList.Add(i.ToString());
        }
        return portList;
    }
}