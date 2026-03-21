using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;
using AspireApp.ApiGateway.LoadBalancing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5127")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Регистрация балансировщика
builder.Services.AddSingleton<ILoadBalancingPolicy, WeightedRandomLoadBalancer>();

builder.Services.AddReverseProxy()
    .LoadFromMemory(new[]
    {
        new RouteConfig
        {
            RouteId = "warehouse-route",
            ClusterId = "warehouse-cluster",
            Match = new RouteMatch { Path = "/warehouse" }
        }
    }, new[]
    {
        new ClusterConfig
        {
            ClusterId = "warehouse-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "replica1", new DestinationConfig { Address = "http://localhost:5001" } },
                { "replica2", new DestinationConfig { Address = "http://localhost:5002" } },
                { "replica3", new DestinationConfig { Address = "http://localhost:5003" } }
            },
            LoadBalancingPolicy = "WeightedRandom"
        }
    });

var app = builder.Build();

app.UseCors();
app.MapReverseProxy();

app.Run();