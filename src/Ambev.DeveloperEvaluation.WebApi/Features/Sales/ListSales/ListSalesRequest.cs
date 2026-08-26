namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Query-string model for listing sales.
/// </summary>
public class ListSalesRequest
{
    /// <summary>1-based page number.</summary>
    public int Page { get; set; } = 1;

    /// <summary>Page size (1-100).</summary>
    public int Size { get; set; } = 10;

    /// <summary>Optional filter by customer external identity.</summary>
    public Guid? CustomerId { get; set; }

    /// <summary>Optional filter by branch external identity.</summary>
    public Guid? BranchId { get; set; }

    /// <summary>Optional filter by cancelled state.</summary>
    public bool? IsCancelled { get; set; }

    /// <summary>Ordering expression, e.g. <c>"saleDate desc"</c>.</summary>
    public string? Order { get; set; }
}
