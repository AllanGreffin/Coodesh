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

        RuleFor(x => x.MaxDate)
            .GreaterThanOrEqualTo(x => x.MinDate!.Value)
            .When(x => x.MinDate.HasValue && x.MaxDate.HasValue)
            .WithMessage("MaxDate must be greater than or equal to MinDate.");
    }
}
