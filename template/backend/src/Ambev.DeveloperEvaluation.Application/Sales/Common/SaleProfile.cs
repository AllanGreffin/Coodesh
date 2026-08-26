using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

/// <summary>
/// Mapping between the <see cref="Sale"/> aggregate and its read models.
/// </summary>
public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<SaleItem, SaleItemResult>();
        CreateMap<Sale, SaleResult>();
    }
}
