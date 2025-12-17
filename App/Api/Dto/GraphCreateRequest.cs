using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

public record GraphCreateRequest(
    NumericRangeDto Range,
    Guid AuthorId,
    List<GraphItemDto> Items
);
