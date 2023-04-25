namespace HotPotato.CLI.Entities;

public class GrafanaAgent
{
    public string GetConfigYaml(List<Service> services)
    {
        return File
            .ReadAllText("./Templates/agent.template")
            .Replace("{{ endpoints_to_scrape }}", 
                $"[ " +
                $"{string.Join(",", services.Select(service => $"{service.Name}:{service.Port}"))}" +
                $" ]");
    }
}