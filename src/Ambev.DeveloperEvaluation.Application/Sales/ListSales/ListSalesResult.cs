using Ambev.DeveloperEvaluation.Application.Sales.Common;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Paged result for <see cref="ListSalesQuery"/>.
/// </summary>
public class ListSalesResult
{
    public IReadOnlyCollection<SaleResult> Items { get; set; } = Array.Empty<SaleResult>();
    public int Page { get; set; }
    public int Size { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => Size <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)Size);
}
