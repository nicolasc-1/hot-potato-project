using System.Text;

namespace HotPotato.CLI.Entities;

public class Stack
{
    public List<Service> Services { get; }
    public ICommunicationNode CommunicationNode { get; }

    public Stack(int serviceCount, ICommunicationNode communicationNode)
    {
        CommunicationNode = communicationNode;
        var portMappings = PortMapping.Get(5176, serviceCount);
        
        Services = new List<Service>();
        for (int i = 0; i < serviceCount; i++)
        {
            Services.Add(new Service($"{CommunicationNode.Endpoint}", portMappings[i]));
        }
    }

    public string BuildComposeTemplate()
    {
        var generatedServices = new StringBuilder();
        Services.ForEach(service => generatedServices.Append(service.ToCompose()));
        generatedServices.Append(CommunicationNode.ToCompose());
        
        string composeTemplate = File.ReadAllText("./Templates/compose.template");
        return composeTemplate.Replace("{{ generated_services }}", generatedServices.ToString());
    }
}