using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

public record GraphResponse(
    Guid Id,
    string Expression,
    string IndependentVariable,
    List<MathPoint> Points,
    NumericRange? Range);
