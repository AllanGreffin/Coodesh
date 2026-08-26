using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Query-string model for listing sales. Follows the conventions in
/// <c>.doc/general-api.md</c>: <c>_page</c>, <c>_size</c>, <c>_order</c> and
/// <c>_minDate</c> / <c>_maxDate</c> for the sale date range.
/// </summary>
public class ListSalesRequest
{
    /// <summary>1-based page number.</summary>
    [FromQuery(Name = "_page")]
    public int Page { get; set; } = 1;

    /// <summary>Page size (1-100).</summary>
    [FromQuery(Name = "_size")]
    public int Size { get; set; } = 10;

    /// <summary>
    /// Ordering expression, e.g. <c>"saleDate desc"</c> or
    /// <c>"totalAmount desc, saleNumber asc"</c>. Direction defaults to ascending.
    /// </summary>
    [FromQuery(Name = "_order")]
    public string? Order { get; set; }

    /// <summary>Optional lower bound (inclusive) for the sale date.</summary>
    [FromQuery(Name = "_minDate")]
    public DateTime? MinDate { get; set; }

    /// <summary>Optional upper bound (inclusive) for the sale date.</summary>
    [FromQuery(Name = "_maxDate")]
    public DateTime? MaxDate { get; set; }

    /// <summary>Optional filter by customer external identity.</summary>
    public Guid? CustomerId { get; set; }

    /// <summary>Optional filter by branch external identity.</summary>
    public Guid? BranchId { get; set; }

    /// <summary>Optional filter by cancelled state.</summary>
    public bool? IsCancelled { get; set; }
}
