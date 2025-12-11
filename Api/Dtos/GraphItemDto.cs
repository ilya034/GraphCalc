using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

public record GraphItemDto(
    Guid Id,
    string Expression,
    string IndependentVariable,
    bool IsVisible,
    NumericRange? Range = null,
    List<MathPoint>? Points = null);