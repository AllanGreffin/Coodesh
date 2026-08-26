using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when a new sale is registered.
/// </summary>
public class SaleCreatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the sale that was created.
    /// </summary>
    public Sale Sale { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SaleCreatedEvent(Sale sale)
    {
        Sale = sale;
    }
}
