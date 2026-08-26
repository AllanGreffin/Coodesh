using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Generates <see cref="CreateSaleCommand"/> instances for the handler tests.
/// </summary>
public static class CreateSaleHandlerTestData
{
    private static readonly Faker<CreateSaleItemCommand> ItemFaker = new Faker<CreateSaleItemCommand>()
        .RuleFor(i => i.ProductId, _ => Guid.NewGuid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 500));

    private static readonly Faker<CreateSaleCommand> CommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(c => c.SaleNumber, f => f.Random.Replace("SALE-#####"))
        .RuleFor(c => c.SaleDate, f => f.Date.Recent())
        .RuleFor(c => c.CustomerId, _ => Guid.NewGuid())
        .RuleFor(c => c.CustomerName, f => f.Company.CompanyName())
        .RuleFor(c => c.BranchId, _ => Guid.NewGuid())
        .RuleFor(c => c.BranchName, f => f.Address.City())
        .RuleFor(c => c.Items, _ => ItemFaker.Generate(2));

    public static CreateSaleCommand GenerateValidCommand() => CommandFaker.Generate();
}
