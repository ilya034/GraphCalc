namespace GraphCalc.Domain.ValueObjects;

public record NumericRange(double Min, double Max, double Step = 1.0);