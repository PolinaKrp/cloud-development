using Microsoft.AspNetCore.Http;
using Ocelot.LoadBalancer.Interfaces;
using Ocelot.Responses;
using Ocelot.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspireApp.ApiGateway.LoadBalancing;

/// <summary>
/// Weighted Random балансировщик нагрузки.
/// Распределяет запросы между сервисами по заданным весам.
/// </summary>
public class WeightedRandomLoadBalancer(List<Service> services) : ILoadBalancer
{
    private readonly List<Service> _services = services;
    private readonly List<int> _weights = services.Select(s => GetWeight(s)).ToList();
    private readonly Random _random = new();

    private static int GetWeight(Service service)
    {
        var port = service.HostAndPort.DownstreamPort;
        return port == 5001 ? 5 : port == 5002 ? 3 : port == 5003 ? 2 : 1;
    }

    public async Task<Response<ServiceHostAndPort>> LeaseAsync(HttpContext httpContext)
    {
        var totalWeight = _weights.Sum();
        var randomValue = _random.Next(totalWeight);

        var current = 0;
        for (var i = 0; i < _services.Count; i++)
        {
            current += _weights[i];
            if (randomValue < current)
            {
                return new OkResponse<ServiceHostAndPort>(_services[i].HostAndPort);
            }
        }

        return new OkResponse<ServiceHostAndPort>(_services.First().HostAndPort);
    }

    public void Release(ServiceHostAndPort hostAndPort) {}

    public string Name => nameof(WeightedRandomLoadBalancer);
    public string Type => "WeightedRandom";
}