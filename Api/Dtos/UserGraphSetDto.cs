using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

public record UserGraphSetDto(
    Guid Id,
    string Title,
    string? Description = null,
    NumericRange? GlobalRange = null,
    List<GraphItemDto>? Items = null);