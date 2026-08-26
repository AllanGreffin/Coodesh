using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Unit tests for the <see cref="Sale"/> aggregate: totals, item consolidation,
/// cancellation behaviour and the domain events it raises.
/// </summary>
public class SaleTests
{
    [Fact(DisplayName = "Creating a sale totals the items and raises SaleCreatedEvent")]
    public void Given_ValidItems_When_SaleCreated_Then_TotalMatchesAndEventRaised()
    {
        var itemA = SaleTestData.GenerateItemData(quantity: 4, unitPrice: 100m);   // 400 - 10% = 360
        var itemB = SaleTestData.GenerateItemData(quantity: 2, unitPrice: 50m);    // 100 - 0%  = 100

        var sale = SaleTestData.GenerateValidSale(itemA, itemB);

        sale.Items.Should().HaveCount(2);
        sale.TotalAmount.Should().Be(460m);
        sale.DomainEvents.OfType<SaleCreatedEvent>().Should().ContainSingle();
    }

    [Fact(DisplayName = "A sale cannot be created without items")]
    public void Given_NoItems_When_SaleCreated_Then_ThrowsDomainException()
    {
        var act = () => Sale.Create("SALE-1", DateTime.UtcNow, Guid.NewGuid(), "ACME",
            Guid.NewGuid(), "Branch", Array.Empty<(Guid, string, int, decimal)>());

        act.Should().Throw<DomainException>().WithMessage("*at least one item*");
    }

    [Fact(DisplayName = "The same product added twice is consolidated into a single item")]
    public void Given_DuplicatedProduct_When_SaleCreated_Then_QuantitiesAreMerged()
    {
        var productId = Guid.NewGuid();
        var first = SaleTestData.GenerateItemData(quantity: 3, unitPrice: 10m, productId: productId);
        var second = SaleTestData.GenerateItemData(quantity: 3, unitPrice: 10m, productId: productId);

        var sale = SaleTestData.GenerateValidSale(first, second);

        sale.Items.Should().ContainSingle();
        var item = sale.Items.Single();
        item.Quantity.Should().Be(6);
        item.Discount.Should().Be(6m); // 60 * 10%
        sale.TotalAmount.Should().Be(54m);
    }

    [Fact(DisplayName = "Cancelling an item recalculates the total and raises ItemCancelledEvent")]
    public void Given_SaleWithTwoItems_When_ItemCancelled_Then_TotalDropsAndEventRaised()
    {
        var itemA = SaleTestData.GenerateItemData(quantity: 2, unitPrice: 100m); // 200
        var itemB = SaleTestData.GenerateItemData(quantity: 1, unitPrice: 30m);  // 30
        var sale = SaleTestData.GenerateValidSale(itemA, itemB);
        var target = sale.Items.First();

        sale.CancelItem(target.Id);

        target.IsCancelled.Should().BeTrue();
        sale.IsCancelled.Should().BeFalse();
        sale.TotalAmount.Should().Be(30m);
        sale.DomainEvents.OfType<ItemCancelledEvent>().Should().ContainSingle();
    }

    [Fact(DisplayName = "Cancelling an unknown item throws a DomainException")]
    public void Given_Sale_When_CancellingUnknownItem_Then_ThrowsDomainException()
    {
        var sale = SaleTestData.GenerateValidSale();

        var act = () => sale.CancelItem(Guid.NewGuid());

        act.Should().Throw<DomainException>();
    }

    [Fact(DisplayName = "Cancelling the sale cancels every item and raises SaleCancelledEvent")]
    public void Given_ActiveSale_When_Cancelled_Then_AllItemsCancelledAndEventRaised()
    {
        var sale = SaleTestData.GenerateValidSale(
            SaleTestData.GenerateItemData(quantity: 5, unitPrice: 20m),
            SaleTestData.GenerateItemData(quantity: 1, unitPrice: 10m));

        sale.Cancel();

        sale.IsCancelled.Should().BeTrue();
        sale.Items.Should().OnlyContain(i => i.IsCancelled);
        sale.TotalAmount.Should().Be(0m);
        sale.DomainEvents.OfType<SaleCancelledEvent>().Should().ContainSingle();
    }

    [Fact(DisplayName = "A cancelled sale cannot be cancelled again")]
    public void Given_CancelledSale_When_CancelledAgain_Then_ThrowsDomainException()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        var act = () => sale.Cancel();

        act.Should().Throw<DomainException>();
    }

    [Fact(DisplayName = "A cancelled sale cannot be updated")]
    public void Given_CancelledSale_When_Updated_Then_ThrowsDomainException()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        var act = () => sale.Update(DateTime.UtcNow, Guid.NewGuid(), "ACME", Guid.NewGuid(), "Branch",
            new[] { SaleTestData.GenerateItemData() });

        act.Should().Throw<DomainException>();
    }

    [Fact(DisplayName = "Updating a sale replaces its items and raises a single SaleModifiedEvent")]
    public void Given_Sale_When_Updated_Then_ItemsReplacedAndEventRaised()
    {
        var sale = SaleTestData.GenerateValidSale(SaleTestData.GenerateItemData(quantity: 1, unitPrice: 10m));
        sale.ClearDomainEvents();

        var newItem = SaleTestData.GenerateItemData(quantity: 10, unitPrice: 100m); // 1000 - 20% = 800
        sale.Update(DateTime.UtcNow, Guid.NewGuid(), "New Customer", Guid.NewGuid(), "New Branch", new[] { newItem });

        sale.Items.Should().ContainSingle();
        sale.TotalAmount.Should().Be(800m);
        sale.CustomerName.Should().Be("New Customer");
        sale.DomainEvents.OfType<SaleModifiedEvent>().Should().ContainSingle();
    }
}