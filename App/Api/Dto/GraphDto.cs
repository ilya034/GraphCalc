using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

public record GraphDto(
    Guid Id,
    NumericRangeDto Range,
    Guid AuthorId,
    List<GraphItemDto> Items
);