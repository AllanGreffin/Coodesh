using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when a single item of a sale is cancelled while the sale itself remains active.
/// </summary>
public class ItemCancelledEvent : IDomainEvent
{
    /// <summary>
    /// Gets the sale that owns the cancelled item.
    /// </summary>
    public Sale Sale { get; }

    /// <summary>
    /// Gets the item that was cancelled.
    /// </summary>
    public SaleItem Item { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public ItemCancelledEvent(Sale sale, SaleItem item)
    {
        Sale = sale;
        Item = item;
    }
}
