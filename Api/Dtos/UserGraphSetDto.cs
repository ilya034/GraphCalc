namespace GraphCalc.Api.Dtos;

public record UserGraphSetDto(
    Guid Id,
    string Title,
    string? Description = null,
    List<UserGraphDto>? Graphs = null);