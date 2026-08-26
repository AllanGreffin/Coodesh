namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Request model for updating an existing sale. The identifier comes from the route.
/// </summary>
public class UpdateSaleRequest
{
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;

    /// <summary>The complete set of items the sale should have after the update.</summary>
    public List<UpdateSaleItemRequest> Items { get; set; } = new();
}

/// <summary>
/// A single item line for an updated sale.
/// </summary>
public class UpdateSaleItemRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
