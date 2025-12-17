namespace GraphCalc.Api.Dtos;

public record NumericRangeDto(
    double Min, 
    double Max, 
    double Step
);