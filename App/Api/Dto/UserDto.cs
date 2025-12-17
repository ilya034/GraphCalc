namespace GraphCalc.Api.Dtos;

public record UserDto(
    Guid Id,
    string Username,
    string Email
);