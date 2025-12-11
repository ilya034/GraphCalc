using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

// DTO для описания графика (без точек)
public record GraphDefinitionDto(
    Guid Id,
    string Expression
);