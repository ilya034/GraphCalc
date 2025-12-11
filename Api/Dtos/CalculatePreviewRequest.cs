using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

public record CalculatePreviewRequest(
    string Expression,
    NumericRange Range
);