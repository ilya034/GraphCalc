using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

public record CalculateSetRequest(
    NumericRange Range
);