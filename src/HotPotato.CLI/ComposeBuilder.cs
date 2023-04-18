using ComposeBuilderDotNet.Builders;
using ComposeBuilderDotNet.Extensions;
using HotPotato.CLI.Entities;

namespace HotPotato.CLI;

public static class ComposeBuilder
{
    public static void Generate(Stack stack)
    {
        var network = Builder.MakeNetwork("hot-potato-network").Build();

        var compose = Builder.MakeCompose()
            .WithNetworks(network);

        foreach (var container in stack.ApiContainers.Select((value, index) => new { index, value }))
        {
            compose.WithServices(
                Builder.MakeService($"player-{container.index}")
                    .WithImage("hot-potato:latest")
                    .WithNetworks(network)
                    .WithPortMapping(PortMapping.Generate())
                    .Build());
        }

        var composeBuilt = compose.Build();
        File.WriteAllText("docker-compose.yaml", composeBuilt.Serialize());
    }
}