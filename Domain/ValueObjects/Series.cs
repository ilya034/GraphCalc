namespace GraphCalc.Domain.ValueObjects;

public record Series(
    string Expression,
    IEnumerable<MathPoint> Points
);