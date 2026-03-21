var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("RedisCache").WithRedisInsight(containerName: "insight");

var ports = new[] { 5001, 5002, 5003 };

for (var i = 0; i < 3; i++)
{
    var api = builder.AddProject<Projects.AspireApp_ApiService>($"warehouse-api-{i}")
        .WithReference(cache)
        .WithEnvironment("REPLICA_ID", i.ToString())
        .WithHttpEndpoint(port: ports[i], name: $"api-{i}")
        .WaitFor(cache);
}

var gateway = builder.AddProject("api-gateway", "../AspireApp.ApiGateway/AspireApp.ApiGateway.csproj")
    .WithHttpEndpoint(port: 5101, name: "gateway");

builder.AddProject("client-wasm", "../Client.Wasm/Client.Wasm.csproj")
    .WithReference(gateway)
    .WithHttpEndpoint(port: 5127, name: "client")
    .WaitFor(gateway);

builder.Build().Run();