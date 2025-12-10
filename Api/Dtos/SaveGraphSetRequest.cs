namespace GraphCalc.Api.Dtos;

public record SaveGraphSetRequest(
    List<SaveGraphRequest> Graphs,
    Guid UserId,
    string Title = "",
    string? Description = null);