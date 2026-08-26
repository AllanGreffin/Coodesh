using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Validator for <see cref="ListSalesRequest"/>.
/// </summary>
public class ListSalesRequestValidator : AbstractValidator<ListSalesRequest>
{
    public ListSalesRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than zero.");
        RuleFor(x => x.Size).InclusiveBetween(1, 100).WithMessage("Size must be between 1 and 100.");
    }
}
