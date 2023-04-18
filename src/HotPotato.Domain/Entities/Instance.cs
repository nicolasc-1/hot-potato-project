namespace HotPotato.Domain.Entities;

public class Instance
{
    public string Name { get; init; } = "";

    public static Instance FromEnvironmentVariable()
    {
        return new Instance
        {
            Name = Environment.GetEnvironmentVariable("INSTANCE_NAME") ?? ""
        };
    }
}