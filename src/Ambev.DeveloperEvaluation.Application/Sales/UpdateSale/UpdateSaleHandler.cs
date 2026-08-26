using Ambev.DeveloperEvaluation.Application.Common.Events;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for <see cref="UpdateSaleCommand"/>.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper _mapper;

    public UpdateSaleHandler(ISaleRepository saleRepository, IEventPublisher eventPublisher, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
    }

    public async Task<SaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale is null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        sale.Update(
            command.SaleDate,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName,
            command.Items.Select(i => (i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)));

        await _saleRepository.UpdateAsync(sale, cancellationToken);
        await sale.DispatchEventsAsync(_eventPublisher, cancellationToken);

        return _mapper.Map<SaleResult>(sale);
    }
}
