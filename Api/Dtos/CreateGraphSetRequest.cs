using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

public record CreateGraphSetRequest(
    string Title,
    List<string> Expressions,
    string? Description = null
);