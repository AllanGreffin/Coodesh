using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Validator for <see cref="ListSalesRequest"/>.
/// </summary>
public class ListSalesRequestValidator : AbstractValidator<ListSalesRequest>
{
    public ListSalesRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("_page must be greater than zero.");
        RuleFor(x => x.Size).InclusiveBetween(1, 100).WithMessage("_size must be between 1 and 100.");

        RuleFor(x => x.MaxDate)
            .GreaterThanOrEqualTo(x => x.MinDate!.Value)
            .When(x => x.MinDate.HasValue && x.MaxDate.HasValue)
            .WithMessage("_maxDate must be greater than or equal to _minDate.");
    }
}
