using System;
using AspireApp.ApiService.Properties.Entities;
namespace AspireApp.ApiService.Properties.Generator;

/// <summary>
/// Интерфейс для запуска юзкейса
/// </summary>
public interface IWarehouseGeneratorService
{
    /// <summary>
    /// Обработка запроса на генерацию товара
    /// </summary>
    public Task<Warehouse> ProcessWarehouse(int id);
}
