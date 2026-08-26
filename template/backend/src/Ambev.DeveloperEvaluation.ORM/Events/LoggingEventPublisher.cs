using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.ORM.Events;

/// <summary>
/// Default <see cref="IEventPublisher"/> implementation.
/// </summary>
/// <remarks>
/// The evaluation does not require publishing to a real message broker, so this
/// implementation simply writes a structured message to the application log. The
/// contract is broker-agnostic, so replacing this with Rebus/MassTransit later only
/// means registering another implementation in the IoC layer.
/// </remarks>
public class LoggingEventPublisher : IEventPublisher
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly ILogger<LoggingEventPublisher> _logger;

    public LoggingEventPublisher(ILogger<LoggingEventPublisher> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(BuildPayload(domainEvent), SerializerOptions);

        _logger.LogInformation(
            "Domain event published: {EventType} at {OccurredOn:o} | {EventPayload}",
            domainEvent.GetType().Name,
            domainEvent.OccurredOn,
            payload);

        return Task.CompletedTask;
    }

    private static object BuildPayload(IDomainEvent domainEvent) => domainEvent switch
    {
        SaleCreatedEvent e => SaleSummary(e.Sale),
        SaleModifiedEvent e => SaleSummary(e.Sale),
        SaleCancelledEvent e => SaleSummary(e.Sale),
        ItemCancelledEvent e => new
        {
            saleId = e.Sale.Id,
            saleNumber = e.Sale.SaleNumber,
            itemId = e.Item.Id,
            productId = e.Item.ProductId,
            productName = e.Item.ProductName,
            quantity = e.Item.Quantity
        },
        _ => new { type = domainEvent.GetType().Name }
    };

    private static object SaleSummary(Sale sale) => new
    {
        saleId = sale.Id,
        saleNumber = sale.SaleNumber,
        customerId = sale.CustomerId,
        branchId = sale.BranchId,
        totalAmount = sale.TotalAmount,
        isCancelled = sale.IsCancelled,
        itemCount = sale.Items.Count
    };
}
