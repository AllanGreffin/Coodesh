namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Abstraction for publishing domain events to the outside world.
/// </summary>
/// <remarks>
/// The evaluation does not require an actual message broker, so the default
/// implementation only writes to the application log. Swapping it for a real
/// bus (Rebus, MassTransit, etc.) is a matter of providing another implementation.
/// </remarks>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes a single domain event.
    /// </summary>
    /// <param name="domainEvent">The event instance to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
