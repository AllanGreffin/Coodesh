using Ambev.DeveloperEvaluation.Application.Sales.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Command for updating an existing sale (header data and items).
/// </summary>
public class UpdateSaleCommand : IRequest<SaleResult>
{
    /// <summary>Identifier of the sale to update.</summary>
    public Guid Id { get; set; }

    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;

    /// <summary>The complete set of items the sale should have after the update.</summary>
    public List<UpdateSaleItemCommand> Items { get; set; } = new();
}

/// <summary>
/// A single line for an updated sale.
/// </summary>
public class UpdateSaleItemCommand
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
