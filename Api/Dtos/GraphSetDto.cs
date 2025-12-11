using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

// DTO для метаданных набора (хранение)
public record GraphSetDto(
    Guid Id,
    Guid AuthorId,
    List<GraphDefinitionDto> Expressions
);