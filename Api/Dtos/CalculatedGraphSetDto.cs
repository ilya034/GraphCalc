using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

public record CalculatedGraphSetDto(
    Guid Id,
    string Title,
    NumericRange Range,
    List<CalculatedGraphDto> Graphs
);