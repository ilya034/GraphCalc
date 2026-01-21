using GraphCalc.Api.Dtos;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.App;

internal static class MathPointMappingExtensions
{
    public static MathPoint ToDomain(this PointDto dto)
        => new MathPoint(dto.X, dto.Y);

    public static PointDto ToDto(this MathPoint domain)
        => new PointDto(domain.X, domain.Y);

    public static IEnumerable<MathPoint> ToDomain(this IEnumerable<PointDto> dtos)
        => dtos.Select(dto => dto.ToDomain());

    public static IEnumerable<PointDto> ToDto(this IEnumerable<MathPoint> domains)
        => domains.Select(domain => domain.ToDto());
}
