using GraphCalc.Api.Dtos;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.App;

public static class GraphItemMappingExtensions
{
    public static GraphItem ToDomain(this GraphItemDto dto)
        => new GraphItem(dto.Expression, dto.IsVisible);

    public static GraphItemDto ToDto(this GraphItem domain)
        => new GraphItemDto(domain.Expression, domain.IsVisible);

    public static IEnumerable<GraphItem> ToDomain(this IEnumerable<GraphItemDto> dtos)
        => dtos.Select(dto => dto.ToDomain());

    public static IEnumerable<GraphItemDto> ToDto(this IEnumerable<GraphItem> domains)
        => domains.Select(domain => domain.ToDto());
}
