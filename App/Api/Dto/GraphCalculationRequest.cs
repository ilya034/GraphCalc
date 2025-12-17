using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

public record GraphCalculationRequest(
    string Expression = "", 
    NumericRange? Range = null
);