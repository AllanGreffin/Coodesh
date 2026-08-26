using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Validator for <see cref="ListSalesQuery"/>.
/// </summary>
public class ListSalesValidator : AbstractValidator<ListSalesQuery>
{
    public ListSalesValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than zero.");
        RuleFor(x => x.Size).InclusiveBetween(1, 100).WithMessage("Size must be between 1 and 100.");
    }
}
