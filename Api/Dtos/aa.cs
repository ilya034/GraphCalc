using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

// --- Requests ---

public record CreateGraphSetRequest(
string Title,
List<string> Expressions,
string? Description = null
);

public record CalculatePreviewRequest(
string Expression,
NumericRange Range
);

public record CalculateSetRequest(
NumericRange Range
);

// --- Responses ---

// DTO для описания графика (без точек)
public record GraphDefinitionDto(
Guid Id,
string Expression
);

// DTO для метаданных набора (хранение)
public record GraphSetDto(
Guid Id,
Guid AuthorId,
string Title,
string? Description,
List<GraphDefinitionDto> Expressions
);

// DTO для отображения (с точками)
public record CalculatedGraphDto(
Guid Id,
string Expression,
List<MathPoint> Points
);

public record CalculatedGraphSetDto(
Guid Id,
string Title,
NumericRange Range,
List<CalculatedGraphDto> Graphs
);