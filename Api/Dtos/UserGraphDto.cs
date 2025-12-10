namespace GraphCalc.Api.Dtos;

public record UserGraphDto(
    Guid Id,
    string Expression,
    string Title,
    string? Description = null);