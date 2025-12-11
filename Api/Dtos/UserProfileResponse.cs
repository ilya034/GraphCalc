namespace GraphCalc.Api.Dtos;

public record UserProfileResponse(
    Guid Id,
    string Username,
    string Email,
    string? Description = null,
    int GraphSetsCount = 0);