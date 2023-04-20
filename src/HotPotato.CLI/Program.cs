using HotPotato.CLI.Entities;

var proxy = new EnvoyProxy();
var stack = new Stack(5, proxy);
var composeFile = stack.BuildComposeTemplate();
File.WriteAllText("./output/docker-compose.stack.yaml", composeFile);
File.WriteAllText("./output/envoy.yaml", proxy.GetConfigYaml(stack.PortMappings));