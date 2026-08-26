using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Centralizes test data generation for the <see cref="Sale"/> aggregate and its items,
/// using the Bogus library.
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Builds a raw item tuple as consumed by <see cref="Sale.Create"/> / <see cref="Sale.Update"/>.
    /// </summary>
    public static (Guid ProductId, string ProductName, int Quantity, decimal UnitPrice) GenerateItemData(
        int? quantity = null,
        decimal? unitPrice = null,
        Guid? productId = null)
    {
        var faker = new Faker();
        return (
            productId ?? Guid.NewGuid(),
            faker.Commerce.ProductName(),
            quantity ?? faker.Random.Int(1, 3),
            unitPrice ?? faker.Random.Decimal(1, 100));
    }

    /// <summary>
    /// Creates a valid sale with the supplied items (or a single random item when none are given).
    /// </summary>
    public static Sale GenerateValidSale(
        params (Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)[] items)
    {
        var faker = new Faker();
        var effectiveItems = items.Length > 0 ? items : new[] { GenerateItemData() };

        return Sale.Create(
            faker.Random.Replace("SALE-#####"),
            faker.Date.Recent(),
            Guid.NewGuid(),
            faker.Company.CompanyName(),
            Guid.NewGuid(),
            faker.Address.City(),
            effectiveItems);
    }

    /// <summary>
    /// Creates a single <see cref="SaleItem"/> with the given quantity and unit price.
    /// </summary>
    public static SaleItem GenerateItem(int quantity, decimal unitPrice = 10m)
        => new(Guid.NewGuid(), new Faker().Commerce.ProductName(), quantity, unitPrice);
}
