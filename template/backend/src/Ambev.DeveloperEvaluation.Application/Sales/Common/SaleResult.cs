namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

/// <summary>
/// Read model for a sale, shared by the create, get, list, update and cancel operations.
/// </summary>
public class SaleResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyCollection<SaleItemResult> Items { get; set; } = Array.Empty<SaleItemResult>();
}
