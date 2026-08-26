using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Command for registering a new sale.
/// </summary>
public class CreateSaleCommand : IRequest<SaleResult>
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
    public List<CreateSaleItemCommand> Items { get; set; } = new();

    public ValidationResultDetail Validate()
    {
        var validator = new CreateSaleCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}

/// <summary>
/// A single line requested for a new sale.
/// </summary>
public class CreateSaleItemCommand
{
    /// <summary>External identity of the product.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Denormalized product description.</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>Amount of identical units.</summary>
    public int Quantity { get; set; }

    /// <summary>Unit price at the moment of the sale.</summary>
    public decimal UnitPrice { get; set; }
}
