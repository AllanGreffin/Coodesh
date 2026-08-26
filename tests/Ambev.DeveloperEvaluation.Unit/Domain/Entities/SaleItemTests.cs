using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Unit tests for the <see cref="SaleItem"/> entity, focused on the quantity-based
/// discount rules defined by the challenge.
/// </summary>
public class SaleItemTests
{
    [Theory(DisplayName = "Discount tier is applied according to the quantity")]
    [InlineData(1, 0)]
    [InlineData(3, 0)]
    [InlineData(4, 10)]
    [InlineData(9, 10)]
    [InlineData(10, 20)]
    [InlineData(20, 20)]
    public void Given_Quantity_When_ItemCreated_Then_AppliesExpectedDiscountRate(int quantity, int expectedRatePercent)
    {
        const decimal unitPrice = 100m;
        var item = SaleTestData.GenerateItem(quantity, unitPrice);

        var gross = unitPrice * quantity;
        var expectedDiscount = gross * expectedRatePercent / 100m;

        item.Discount.Should().Be(expectedDiscount);
        item.TotalAmount.Should().Be(gross - expectedDiscount);
    }

    [Fact(DisplayName = "Quantities below 4 are not eligible for a discount")]
    public void Given_QuantityBelowFour_When_ItemCreated_Then_HasNoDiscount()
    {
        var item = SaleTestData.GenerateItem(3, 50m);

        item.Discount.Should().Be(0m);
        item.TotalAmount.Should().Be(150m);
    }

    [Fact(DisplayName = "Selling more than 20 identical items is not allowed")]
    public void Given_QuantityAboveTwenty_When_ItemCreated_Then_ThrowsDomainException()
    {
        var act = () => SaleTestData.GenerateItem(21);

        act.Should().Throw<DomainException>()
            .WithMessage("*more than 20 identical items*");
    }

    [Fact(DisplayName = "Quantity must be greater than zero")]
    public void Given_NonPositiveQuantity_When_ItemCreated_Then_ThrowsDomainException()
    {
        var act = () => SaleTestData.GenerateItem(0);

        act.Should().Throw<DomainException>();
    }

    [Fact(DisplayName = "Cancelling an item zeroes its discount and total")]
    public void Given_ActiveItem_When_Cancelled_Then_TotalsBecomeZero()
    {
        var item = SaleTestData.GenerateItem(10, 100m);
        item.TotalAmount.Should().BeGreaterThan(0m);

        item.Cancel();

        item.IsCancelled.Should().BeTrue();
        item.Discount.Should().Be(0m);
        item.TotalAmount.Should().Be(0m);
    }

    [Fact(DisplayName = "Increasing quantity re-evaluates the discount tier")]
    public void Given_Item_When_QuantityIncreasedIntoHigherTier_Then_DiscountRecalculated()
    {
        var item = SaleTestData.GenerateItem(3, 100m);
        item.Discount.Should().Be(0m);

        item.IncreaseQuantity(7); // now 10 units -> 20%

        item.Quantity.Should().Be(10);
        item.Discount.Should().Be(200m);
        item.TotalAmount.Should().Be(800m);
    }
}
