namespace GraphCalc.Api.Dtos;

public record SaveGraphRequest(
    string Expression = "",
    double XMin = 0,
    double XMax = 0,
    double XStep = 0,
    bool AutoYRange = false,
    Guid UserId = default,
    string Title = "",
    string? Description = null);
