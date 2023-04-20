using Bogus;
using HotPotato.Domain.Entities;

namespace HotPotato.CLI.Entities;

public class Service
{
    public string Name { get; set; }
    public string Mode { get; set; }
    public string Endpoint { get; set; }

    public Service(string endpoint)
    {
        Name = new Faker<Instance>()
            .RuleFor(i => i.Name, f => f.Name.FirstName().ToLower())
            .Generate()
            .Name;
        Mode = "Http";
        Endpoint = endpoint;
    }

    public string ToCompose(string portNumber)
    {
        return @$"
  {Name}:
    build: 
      context: ./src
      dockerfile: HotPotatoApi.Dockerfile
    container_name: hotpotato-{Name}
    ports:
      - ""{portNumber}:80""
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