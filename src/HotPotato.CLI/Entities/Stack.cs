namespace HotPotato.CLI.Entities;

public class Stack
{
    public List<ApiContainer> ApiContainers { get; set; } = new List<ApiContainer>();
    public LoadBalancer LoadBalancer { get; set; } = new LoadBalancer();

    public static Stack FromDefault()
    {
        return new Stack
        {
            ApiContainers = new List<ApiContainer>
            {
                ApiContainer.FromDefault(),
                ApiContainer.FromDefault()
            },
            LoadBalancer = LoadBalancer.FromDefault()
        };
    }
}