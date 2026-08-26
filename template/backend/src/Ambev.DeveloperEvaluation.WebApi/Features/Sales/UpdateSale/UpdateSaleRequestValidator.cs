using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Validator for <see cref="UpdateSaleRequest"/>.
/// </summary>
public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(x => x.SaleDate).NotEmpty().WithMessage("Sale date is required.");
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.BranchName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("A sale must have at least one item.");

        RuleForEach(x => x.Items).SetValidator(new UpdateSaleItemRequestValidator());
    }
}

/// <summary>
/// Validator for a single <see cref="UpdateSaleItemRequest"/>.
/// </summary>
public class UpdateSaleItemRequestValidator : AbstractValidator<UpdateSaleItemRequest>
{
    public UpdateSaleItemRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(SaleItem.MaxQuantity)
            .WithMessage($"It is not possible to sell more than {SaleItem.MaxQuantity} identical items.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
    }
}
