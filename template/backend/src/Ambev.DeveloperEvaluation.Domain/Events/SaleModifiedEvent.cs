using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when an existing sale has its data changed (header or items).
/// </summary>
public class SaleModifiedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the sale that was modified.
    /// </summary>
    public Sale Sale { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SaleModifiedEvent(Sale sale)
    {
        Sale = sale;
    }
}
