using Bogus;
using HotPotato.Domain.Entities;

namespace HotPotato.CLI.Entities;

public class ApiContainer
{
    public string Name { get; set; } = "";

    public static ApiContainer FromDefault()
    {
        return new ApiContainer
        {
            Name = new Faker<Instance>()
                .RuleFor(i => i.Name, f => f.Name.FirstName())
                .Generate()
                .Name
        };
    }
}