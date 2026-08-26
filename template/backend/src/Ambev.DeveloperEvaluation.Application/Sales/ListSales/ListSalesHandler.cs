using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for <see cref="ListSalesQuery"/>.
/// </summary>
public class ListSalesHandler : IRequestHandler<ListSalesQuery, ListSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<ListSalesResult> Handle(ListSalesQuery request, CancellationToken cancellationToken)
    {
        var validator = new ListSalesValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var filter = new SaleListFilter
        {
            Page = request.Page,
            Size = request.Size,
            CustomerId = request.CustomerId,
            BranchId = request.BranchId,
            IsCancelled = request.IsCancelled,
            Order = request.Order
        };

        var (sales, totalCount) = await _saleRepository.ListAsync(filter, cancellationToken);

        return new ListSalesResult
        {
            Items = sales.Select(_mapper.Map<SaleResult>).ToArray(),
            Page = request.Page,
            Size = request.Size,
            TotalItems = totalCount
        };
    }
}
