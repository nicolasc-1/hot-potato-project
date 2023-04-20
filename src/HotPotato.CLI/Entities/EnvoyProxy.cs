using System.Text;

namespace HotPotato.CLI.Entities;

public class EnvoyProxy : ICommunicationNode
{
    public string Image { get; set; } = "envoyproxy/envoy:v1.26-latest";
    public string Port { get; set; } = "10000";
    public string Endpoint => $"envoy:{Port}";

    public string GetConfigYaml(List<string> portMappings)
    {
        string config = File.ReadAllText("./Templates/envoy.template");
        
        var configYaml = new StringBuilder();
        foreach (var portMapping in portMappings)
        {
            configYaml.Append(
                $@"
          - lb_endpoints:
            - endpoint:
                address:
                  socket_address:
                    address: host.docker.internal
                    port_value: {portMapping}
");
        }

        return config
            .Replace("{{ generated_endpoints }}", configYaml.ToString())
            .Replace("{{ port }}", Port);
    }

    public string ToCompose()
    {
        return @$"
  envoy:
    image: {Image}
    container_name: envoy
    ports:
      - '{Port}:{Port}'
      - '9901:9901'
    command: [ '/usr/local/bin/envoy', '-c', '/etc/envoy/envoy.yaml', '--log-level', 'debug' ]
    volumes:
      - ./envoy.yaml:/etc/envoy/envoy.yaml
    networks:
      - hot_potato_network";
    }
}