using AspireApp.ApiService.Properties.Generator; 
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisDistributedCache("RedisCache");

builder.Services.AddScoped<IWarehouseCache, WarehouseCache>();
builder.Services.AddScoped<WarehouseGenerator>();
builder.Services.AddScoped<IWarehouseGeneratorService, WarehouseGeneratorService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/warehouse", async (IWarehouseGeneratorService service, int id) =>
{
    var warehouse = await service.ProcessWarehouse(id);
    return Results.Ok(warehouse);
});

app.UseCors();

app.Run();