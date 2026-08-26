namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Optional filters and paging information consumed by <see cref="ISaleRepository.ListAsync"/>.
/// </summary>
public class SaleListFilter
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public Guid? CustomerId { get; set; }
    public Guid? BranchId { get; set; }
    public bool? IsCancelled { get; set; }

    /// <summary>Lower bound (inclusive) for the sale date.</summary>
    public DateTime? MinDate { get; set; }

    /// <summary>Upper bound (inclusive) for the sale date.</summary>
    public DateTime? MaxDate { get; set; }

    /// <summary>Ordering expression, e.g. <c>"saleDate desc, saleNumber asc"</c>.</summary>
    public string? Order { get; set; }
}
