using System.Text;

namespace HotPotato.CLI.Entities;

public class Stack
{
    public List<Service> Services { get; }
    public ICommunicationNode CommunicationNode { get; }
    
    public List<string> PortMappings { get; }

    public Stack(int serviceCount, ICommunicationNode communicationNode)
    {
        CommunicationNode = communicationNode;
        PortMappings = PortMapping.Get(5176, serviceCount);
        
        Services = new List<Service>();
        for (int i = 0; i < serviceCount; i++)
        {
            Services.Add(new Service($"{CommunicationNode.Endpoint}"));
        }
    }

    public string BuildComposeTemplate()
    {
        string composeTemplate = File.ReadAllText("./Templates/compose.template");

        var generatedServices = new StringBuilder();
        for (int i = 0; i < Services.Count; i++)
        {
            generatedServices.Append(Services[i].ToCompose(this.PortMappings[i]));
        }

        generatedServices.Append(CommunicationNode.ToCompose());
        return composeTemplate.Replace("{{ generated_services }}", generatedServices.ToString());
    }

}