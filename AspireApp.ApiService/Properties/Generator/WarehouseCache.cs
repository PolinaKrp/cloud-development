using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using AspireApp.ApiService.Properties.Entities;

namespace AspireApp.ApiService.Properties.Generator;

/// <summary>
/// Сервис для работы с кэшем товаров на складе
/// </summary>
public interface IWarehouseCache
{
    /// <summary>Получить товар из кэша по идентификатору</summary>
    Task<Warehouse?> GetAsync(int id);

    /// <summary>Сохранить товар в кэш с временем</summary>
    Task SetAsync(Warehouse warehouse, TimeSpan expiration);
}

/// <summary>
/// Кэширование товаров 
/// </summary>
public class WarehouseCache : IWarehouseCache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<WarehouseCache> _logger;

    public WarehouseCache(IDistributedCache cache, ILogger<WarehouseCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<Warehouse?> GetAsync(int id)
    {
        var key = $"warehouse_{id}";
        var cached = await _cache.GetStringAsync(key);
        if (cached == null)
            return null;

        try
        {
            return JsonSerializer.Deserialize<Warehouse>(cached);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка десериализации товара {Id} из кэша", id);
            return null;
        }
    }

    public async Task SetAsync(Warehouse warehouse, TimeSpan expiration)
    {
        var key = $"warehouse_{warehouse.Id}";
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };
        var serialized = JsonSerializer.Serialize(warehouse);
        await _cache.SetStringAsync(key, serialized, options);
    }
}