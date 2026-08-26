using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// Mapping for the DeleteSale feature.
/// </summary>
public class DeleteSaleProfile : Profile
{
    public DeleteSaleProfile()
    {
        CreateMap<Guid, Application.Sales.DeleteSale.DeleteSaleCommand>()
            .ConstructUsing(id => new Application.Sales.DeleteSale.DeleteSaleCommand(id));
    }
}
