using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Query for listing sales with paging, filtering and ordering.
/// </summary>
public class ListSalesQuery : IRequest<ListSalesResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;

    /// <summary>Ordering expression, e.g. <c>"saleDate desc"</c> or <c>"totalAmount desc, saleNumber asc"</c>.</summary>
    public string? Order { get; set; }

    /// <summary>Optional lower bound (inclusive) for the sale date.</summary>
    public DateTime? MinDate { get; set; }

    /// <summary>Optional upper bound (inclusive) for the sale date.</summary>
    public DateTime? MaxDate { get; set; }

    /// <summary>Optional filter by customer external identity.</summary>
    public Guid? CustomerId { get; set; }

    /// <summary>Optional filter by branch external identity.</summary>
    public Guid? BranchId { get; set; }

    /// <summary>Optional filter by cancelled state.</summary>
    public bool? IsCancelled { get; set; }
}
