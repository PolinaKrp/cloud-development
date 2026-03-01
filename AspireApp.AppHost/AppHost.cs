var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("RedisCache").WithRedisInsight(containerName: "insight");

var service = builder.AddProject<Projects.AspireApp_ApiService>("service-api")
    .WithReference(cache)
    .WaitFor(cache);

builder.AddProject("client-wasm", "../Client.Wasm/Client.Wasm.csproj")
    .WithReference(service)
    .WaitFor(service);

builder.Build().Run();