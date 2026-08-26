namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Marker interface for domain events raised by aggregates.
/// Events are collected on the entity and dispatched by the application layer
/// after the changes have been persisted.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the moment (UTC) the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }
}
