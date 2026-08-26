using Ambev.DeveloperEvaluation.Application.Sales.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

/// <summary>
/// Mapping between the application read models and the API response models for sales.
/// </summary>
public class SaleResponseProfile : Profile
{
    public SaleResponseProfile()
    {
        CreateMap<SaleItemResult, SaleItemResponse>();
        CreateMap<SaleResult, SaleResponse>();
    }
}
