namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Request model for registering a new sale.
/// </summary>
public class CreateSaleRequest
{
    /// <summary>Human-readable sale number. Must be unique.</summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>Date the sale was made.</summary>
    public DateTime SaleDate { get; set; }

    /// <summary>External identity of the customer.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Denormalized customer description.</summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>External identity of the branch where the sale was made.</summary>
    public Guid BranchId { get; set; }

    /// <summary>Denormalized branch description.</summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>Items that compose the sale.</summary>
    public List<CreateSaleItemRequest> Items { get; set; } = new();
}

/// <summary>
/// A single item line for a new sale.
/// </summary>
public class CreateSaleItemRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
