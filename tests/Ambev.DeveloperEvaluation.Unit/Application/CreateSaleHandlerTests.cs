using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Unit tests for <see cref="CreateSaleHandler"/>.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _handler = new CreateSaleHandler(_saleRepository, _eventPublisher, _mapper);
        _mapper.Map<SaleResult>(Arg.Any<Sale>()).Returns(ci => new SaleResult { Id = ((Sale)ci[0]).Id });
    }

    [Fact(DisplayName = "Given a valid command When handling Then persists the sale and publishes SaleCreatedEvent")]
    public async Task Handle_ValidCommand_PersistsAndPublishes()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>()).Returns((Sale?)null);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(ci => (Sale)ci[0]);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s => s.SaleNumber == command.SaleNumber && s.Items.Count == command.Items.Count),
            Arg.Any<CancellationToken>());
        await _eventPublisher.Received(1).PublishAsync(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a duplicated sale number When handling Then throws InvalidOperationException")]
    public async Task Handle_DuplicatedSaleNumber_Throws()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(SaleTestDataFactory(command.SaleNumber));

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        await _saleRepository.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given an empty command When handling Then throws a validation exception")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        var act = () => _handler.Handle(new CreateSaleCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    private static Sale SaleTestDataFactory(string saleNumber) => Sale.Create(
        saleNumber, DateTime.UtcNow, Guid.NewGuid(), "ACME", Guid.NewGuid(), "Branch",
        [(Guid.NewGuid(), "Product", 1, 10m)]);
}
