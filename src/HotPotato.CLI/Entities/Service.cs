using Bogus;
using HotPotato.Domain.Entities;

namespace HotPotato.CLI.Entities;

public class Service
{
    public string Name { get; set; }
    public string Mode { get; set; }
    public string Endpoint { get; set; }
    public string Port { get; set; }

    public Service(string endpoint, string port)
    {
        Name = new Faker<Instance>()
            .RuleFor(i => i.Name, f => f.Name.FirstName().ToLower())
            .Generate()
            .Name;
        Mode = "Http";
        Endpoint = endpoint;
        Port = port;
    }

    public string ToCompose()
    {
        return @$"
  {Name}:
    build: 
      context: ./src
      dockerfile: HotPotatoApi.Dockerfile
    container_name: hotpotato-{Name}
    ports:
      - ""{Port}:80""
    environment:
      - MODE={Mode}
      - ENDPOINT={Endpoint}
      - INSTANCE_NAME={Name}
    volumes:
      - C:\\logs\\apps:/logs/apps
    networks:
      - hot_potato_network
";
    }
}