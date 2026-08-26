using Ambev.DeveloperEvaluation.Application.Common.Events;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for <see cref="CreateSaleCommand"/>.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper _mapper;

    public CreateSaleHandler(ISaleRepository saleRepository, IEventPublisher eventPublisher, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
    }

    public async Task<SaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existing = await _saleRepository.GetByNumberAsync(command.SaleNumber, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");

        var sale = Sale.Create(
            command.SaleNumber,
            command.SaleDate,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName,
            command.Items.Select(i => (i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)));

        await _saleRepository.CreateAsync(sale, cancellationToken);
        await sale.DispatchEventsAsync(_eventPublisher, cancellationToken);

        return _mapper.Map<SaleResult>(sale);
    }
}
