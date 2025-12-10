namespace GraphCalc.Api.Dtos;

public record UserGraphsListResponse(
    Guid UserId,
    List<UserGraphDto>? Graphs = null,
    List<UserGraphSetDto>? GraphSets = null);