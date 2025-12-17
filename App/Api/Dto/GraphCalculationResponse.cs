using GraphCalc.Domain.ValueObjects;

public record GraphCalculationResponse(
    double[] YValues,
    NumericRange Range,
    string Expression
);