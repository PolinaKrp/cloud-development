using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("RedisCache");

var service = builder.AddProject("service-api", "../AspireApp.ApiService/AspireApp.ApiService.csproj")
    .WithReference(cache);

builder.AddProject("client-wasm", "../Client.Wasm/Client.Wasm.csproj")
    .WithReference(service);

builder.Build().Run();