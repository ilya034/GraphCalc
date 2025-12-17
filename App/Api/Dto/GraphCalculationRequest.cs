namespace GraphCalc.Api.Dtos;

public record GraphCalculationRequest(
    List<GraphItemDto> Items,
    NumericRangeDto Range
);