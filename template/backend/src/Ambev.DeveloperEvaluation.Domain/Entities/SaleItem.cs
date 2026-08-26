using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a single product line within a <see cref="Sale"/>.
/// Each item owns its own quantity, unit price, discount and total amount, and can be
/// cancelled independently from the sale it belongs to.
/// </summary>
/// <remarks>
/// The product is referenced through the External Identities pattern: only the identifier
/// (<see cref="ProductId"/>) is a real cross-domain reference, while <see cref="ProductName"/>
/// is a denormalized copy kept for reading purposes.
/// </remarks>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Maximum amount of identical units allowed in a single item (business rule).
    /// </summary>
    public const int MaxQuantity = 20;

    /// <summary>
    /// Minimum quantity that becomes eligible for a discount (business rule).
    /// </summary>
    public const int MinQuantityForDiscount = 4;

    /// <summary>
    /// Quantity threshold for the higher discount tier (business rule).
    /// </summary>
    public const int HigherDiscountThreshold = 10;

    /// <summary>
    /// Gets the identifier of the owning sale.
    /// </summary>
    public Guid SaleId { get; private set; }

    /// <summary>
    /// Gets the external identity of the sold product.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the denormalized product description.
    /// </summary>
    public string ProductName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the amount of identical units sold in this item.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Gets the unit price captured at the moment of the sale.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Gets the discount amount (absolute value) applied to this item.
    /// </summary>
    public decimal Discount { get; private set; }

    /// <summary>
    /// Gets the net total for this item: <c>(UnitPrice * Quantity) - Discount</c>.
    /// A cancelled item has a total of zero.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this item has been cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItem"/> class. Used by EF Core.
    /// </summary>
    protected SaleItem()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItem"/> class.
    /// </summary>
    /// <param name="productId">External identity of the product.</param>
    /// <param name="productName">Denormalized product description.</param>
    /// <param name="quantity">Amount of identical units.</param>
    /// <param name="unitPrice">Unit price at the moment of the sale.</param>
    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        SetQuantity(quantity);
    }

    /// <summary>
    /// Replaces the quantity and recalculates discount and total.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown when the quantity is not positive or exceeds <see cref="MaxQuantity"/>.
    /// </exception>
    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Sale item quantity must be greater than zero.");

        if (quantity > MaxQuantity)
            throw new DomainException($"It is not possible to sell more than {MaxQuantity} identical items.");

        Quantity = quantity;
        ApplyDiscountRules();
    }

    /// <summary>
    /// Adds units to the current quantity. Used when the same product is added twice to a sale.
    /// </summary>
    public void IncreaseQuantity(int amount) => SetQuantity(Quantity + amount);

    /// <summary>
    /// Updates the unit price and recalculates discount and total.
    /// </summary>
    public void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = unitPrice;
        ApplyDiscountRules();
    }

    /// <summary>
    /// Cancels this item. A cancelled item no longer contributes to the sale total.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled)
            return;

        IsCancelled = true;
        Discount = 0m;
        TotalAmount = 0m;
    }

    /// <summary>
    /// Performs validation of the sale item entity using the <see cref="SaleItemValidator"/> rules.
    /// </summary>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }

    /// <summary>
    /// Applies the quantity-based discount tiers defined by the business rules:
    /// <list type="bullet">
    /// <item><description>from 4 to 9 identical units: 10% discount</description></item>
    /// <item><description>from 10 to 20 identical units: 20% discount</description></item>
    /// <item><description>fewer than 4 units: no discount</description></item>
    /// </list>
    /// </summary>
    private void ApplyDiscountRules()
    {
        if (IsCancelled)
            return;

        var grossAmount = UnitPrice * Quantity;

        var discountRate = Quantity switch
        {
            >= HigherDiscountThreshold => 0.20m,
            >= MinQuantityForDiscount => 0.10m,
            _ => 0m
        };

        Discount = Math.Round(grossAmount * discountRate, 2, MidpointRounding.AwayFromZero);
        TotalAmount = grossAmount - Discount;
    }
}
