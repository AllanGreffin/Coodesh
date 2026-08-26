using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Mapping for the ListSales feature. The <c>SaleResult -&gt; SaleResponse</c> map lives in
/// <see cref="Common.SaleResponseProfile"/>.
/// </summary>
public class ListSalesProfile : Profile
{
    public ListSalesProfile()
    {
        CreateMap<ListSalesRequest, ListSalesQuery>();
    }
}
