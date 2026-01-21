using GraphCalc.Api.Dtos;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.App;

public static class SeriesMappingExtensions
{
    public static Series ToDomain(this GraphSeriesDto dto)
        => new Series(dto.Expression, dto.Points.ToDomain());

    public static GraphSeriesDto ToDto(this Series domain)
        => new GraphSeriesDto(domain.Expression, domain.Points.ToDto().ToList());

    public static IEnumerable<Series> ToDomain(this IEnumerable<GraphSeriesDto> dtos)
        => dtos.Select(dto => dto.ToDomain());

    public static IEnumerable<GraphSeriesDto> ToDto(this IEnumerable<Series> domains)
        => domains.Select(domain => domain.ToDto());
}
