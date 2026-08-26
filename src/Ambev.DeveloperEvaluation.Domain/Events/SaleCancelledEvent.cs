using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when a whole sale is cancelled.
/// </summary>
public class SaleCancelledEvent : IDomainEvent
{
    /// <summary>
    /// Gets the sale that was cancelled.
    /// </summary>
    public Sale Sale { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public SaleCancelledEvent(Sale sale)
    {
        Sale = sale;
    }
}
