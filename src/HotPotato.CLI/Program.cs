using HotPotato.CLI.Entities;

var proxy = new EnvoyProxy
{
    Port = Environment.GetEnvironmentVariable("ENVOY_PORT") ?? "10000"
};

var stack = new Stack(
    int.Parse(Environment.GetEnvironmentVariable("NB_INSTANCES") ?? "5"), 
    proxy);

File.WriteAllText("./output/docker-compose.stack.yml", stack.BuildComposeTemplate());
File.WriteAllText("./output/envoy.yaml", proxy.GetConfigYaml(stack.Services.Select(service => service.Port).ToList()));
File.WriteAllText("./output/agent.yaml", new GrafanaAgent().GetConfigYaml(stack.Services));