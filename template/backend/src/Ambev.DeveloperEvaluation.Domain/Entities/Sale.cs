using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale record. This is the aggregate root: sale items are only created,
/// changed or cancelled through this entity, which keeps the totals consistent and
/// raises the corresponding domain events.
/// </summary>
/// <remarks>
/// Customer and Branch are referenced with the External Identities pattern: only their
/// identifiers cross domain boundaries, while <see cref="CustomerName"/> and
/// <see cref="BranchName"/> are denormalized copies kept for reading purposes.
/// </remarks>
public class Sale : BaseEntity
{
    private readonly List<SaleItem> _items = new();

    /// <summary>
    /// Gets the human-readable sale number.
    /// </summary>
    public string SaleNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the date the sale was made.
    /// </summary>
    public DateTime SaleDate { get; private set; }

    /// <summary>
    /// Gets the external identity of the customer.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Gets the denormalized customer description.
    /// </summary>
    public string CustomerName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the external identity of the branch where the sale was made.
    /// </summary>
    public Guid BranchId { get; private set; }

    /// <summary>
    /// Gets the denormalized branch description.
    /// </summary>
    public string BranchName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the total amount of the sale, i.e. the sum of every non-cancelled item total.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the whole sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Gets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the items that compose the sale.
    /// </summary>
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="Sale"/> class. Used by EF Core.
    /// </summary>
    protected Sale()
    {
    }

    private Sale(string saleNumber, DateTime saleDate, Guid customerId, string customerName, Guid branchId, string branchName)
    {
        SaleNumber = saleNumber;
        SaleDate = saleDate;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new sale with its items and raises <see cref="SaleCreatedEvent"/>.
    /// </summary>
    public static Sale Create(
        string saleNumber,
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        IEnumerable<(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        var sale = new Sale(saleNumber, saleDate, customerId, customerName, branchId, branchName);

        foreach (var item in items)
            sale.AddOrMergeItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);

        sale.EnsureHasItems();
        sale.RecalculateTotal();
        sale.AddDomainEvent(new SaleCreatedEvent(sale));
        return sale;
    }

    /// <summary>
    /// Replaces the sale header data and the whole set of items, then raises a single
    /// <see cref="SaleModifiedEvent"/>.
    /// </summary>
    public void Update(
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        IEnumerable<(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        EnsureNotCancelled();

        SaleDate = saleDate;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;

        _items.Clear();
        foreach (var item in items)
            AddOrMergeItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);

        EnsureHasItems();
        RecalculateTotal();
        Touch();
        AddDomainEvent(new SaleModifiedEvent(this));
    }

    /// <summary>
    /// Cancels a single item and raises <see cref="ItemCancelledEvent"/>.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the item does not belong to the sale.</exception>
    public void CancelItem(Guid itemId)
    {
        EnsureNotCancelled();

        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new DomainException($"Item {itemId} does not belong to sale {SaleNumber}.");

        if (item.IsCancelled)
            throw new DomainException($"Item {itemId} is already cancelled.");

        item.Cancel();
        RecalculateTotal();
        Touch();
        AddDomainEvent(new ItemCancelledEvent(this, item));
    }

    /// <summary>
    /// Cancels the whole sale (and every item that is still active) and raises <see cref="SaleCancelledEvent"/>.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled)
            throw new DomainException($"Sale {SaleNumber} is already cancelled.");

        foreach (var item in _items.Where(i => !i.IsCancelled))
            item.Cancel();

        IsCancelled = true;
        RecalculateTotal();
        Touch();
        AddDomainEvent(new SaleCancelledEvent(this));
    }

    /// <summary>
    /// Performs validation of the sale entity using the <see cref="SaleValidator"/> rules.
    /// </summary>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }

    private void AddOrMergeItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        // The discount rules talk about "identical items", so the same product is
        // consolidated into a single line by summing the quantities.
        var existing = _items.FirstOrDefault(i => i.ProductId == productId && !i.IsCancelled);
        if (existing is not null)
        {
            existing.IncreaseQuantity(quantity);
            return;
        }

        _items.Add(new SaleItem(productId, productName, quantity, unitPrice));
    }

    private void RecalculateTotal()
        => TotalAmount = _items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);

    private void Touch() => UpdatedAt = DateTime.UtcNow;

    private void EnsureHasItems()
    {
        if (_items.Count == 0)
            throw new DomainException("A sale must have at least one item.");
    }

    private void EnsureNotCancelled()
    {
        if (IsCancelled)
            throw new DomainException($"Sale {SaleNumber} is cancelled and cannot be changed.");
    }
}
