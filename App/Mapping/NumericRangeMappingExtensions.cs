using GraphCalc.Api.Dtos;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.App;

public static class NumericRangeMappingExtensions
{
    public static NumericRange ToDomain(this NumericRangeDto dto)
        => new NumericRange(dto.Min, dto.Max, dto.Step);

    public static NumericRangeDto ToDto(this NumericRange domain)
        => new NumericRangeDto(domain.Min, domain.Max, domain.Step);
}
