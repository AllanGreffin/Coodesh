using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for <see cref="CreateSaleCommand"/>.
/// </summary>
/// <remarks>
/// Rules enforced here:
/// - Sale number, customer and branch data are required.
/// - The sale must have at least one item.
/// - Each item quantity must be between 1 and 20 (business rule: no more than 20 identical items).
/// - Unit price must be greater than zero.
/// </remarks>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.SaleNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.SaleDate).NotEmpty().WithMessage("Sale date is required.");
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.BranchName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("A sale must have at least one item.");

        RuleForEach(x => x.Items).SetValidator(new CreateSaleItemCommandValidator());
    }
}

/// <summary>
/// Validator for a single <see cref="CreateSaleItemCommand"/>.
/// </summary>
public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
{
    public CreateSaleItemCommandValidator()
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
