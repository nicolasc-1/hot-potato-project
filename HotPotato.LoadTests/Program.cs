// See https://aka.ms/new-console-template for more information

using HotPotato.Domain.Entities;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

var httpClient = new HttpClient();

var scenario = Scenario.Create("Simple potato throw", async context =>
    {
        var jsonPotato = JsonConvert.SerializeObject(new Potato
        {
            ThrowDelay = 50,
            TimeToLive = 50
        });
        
        var request =
            Http.CreateRequest("POST", "http://localhost:10000/throw")
                .WithHeader("Content-Type", "text/html")
                .WithBody(new StringContent(jsonPotato));

        var response = await Http.Send(httpClient, request);

        return response;
    })
    .WithoutWarmUp()
    .WithLoadSimulations(
        Simulation.Inject(
            rate: 1,
            interval: TimeSpan.FromMilliseconds(1000),
            during: TimeSpan.FromSeconds(1))
    );
    
    NBomberRunner
        .RegisterScenarios(scenario)
        .Run();