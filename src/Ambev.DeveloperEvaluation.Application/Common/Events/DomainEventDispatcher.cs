using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Common.Events;

/// <summary>
/// Helper that dispatches the domain events accumulated by an aggregate and clears them afterwards.
/// It is meant to be called by the handlers right after the changes are persisted.
/// </summary>
public static class DomainEventDispatcher
{
    public static async Task DispatchEventsAsync(
        this BaseEntity aggregate,
        IEventPublisher publisher,
        CancellationToken cancellationToken)
    {
        var domainEvents = aggregate.DomainEvents.ToArray();
        aggregate.ClearDomainEvents();

        foreach (var domainEvent in domainEvents)
            await publisher.PublishAsync(domainEvent, cancellationToken);
    }
}
