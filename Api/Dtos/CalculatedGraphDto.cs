using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

// DTO для отображения (с точками)
public record CalculatedGraphDto(
    Guid Id,
    string Expression,
    List<MathPoint> Points
);