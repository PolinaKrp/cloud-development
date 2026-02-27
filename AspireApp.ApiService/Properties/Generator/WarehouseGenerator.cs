using Bogus;
using AspireApp.ApiService.Properties.Entities;

namespace AspireApp.ApiService.Properties.Generator;

/// <summary>
/// Генератор случайных данных для товаров на складе
/// </summary>
public class WarehouseGenerator
{
    private readonly Faker<Warehouse> _faker;

    public WarehouseGenerator()
    {
        _faker = new Faker<Warehouse>()
            .RuleFor(p => p.Id, f => f.IndexGlobal + 1)
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())  
            .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
            .RuleFor(p => p.StockQuantity, f => f.Random.Int(0, 1000))
            .RuleFor(p => p.Price, f => Math.Round(f.Random.Decimal(1, 10000), 2))
            .RuleFor(p => p.Weight, f => Math.Round(f.Random.Double(0.1, 100), 2))
            .RuleFor(p => p.Dimensions, f =>
                $"{f.Random.Int(1, 100)}х{f.Random.Int(1, 100)}х{f.Random.Int(1, 100)} см")
            .RuleFor(p => p.IsFragile, f => f.Random.Bool(0.3f))
            .RuleFor(p => p.LastDeliveryDate, f => DateOnly.FromDateTime(f.Date.Past(1)))
            .RuleFor(p => p.NextDeliveryDate, (f, p) =>
                DateOnly.FromDateTime(
                    f.Date.Soon(30, p.LastDeliveryDate.ToDateTime(TimeOnly.MinValue))
                ));
    }

    /// <summary>
    /// Генерация случайного товара
    /// </summary>
    public Warehouse Generate() => _faker.Generate();

    /// <summary>
    /// Генерация нескольких случайных товаров
    /// </summary>
    public List<Warehouse> Generate(int count) => _faker.Generate(count);
}