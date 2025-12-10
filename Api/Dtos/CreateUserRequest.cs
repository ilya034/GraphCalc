namespace GraphCalc.Api.Dtos;

public record CreateUserRequest(
    string Username = "",
    string Email = "",
    string? Description = null);