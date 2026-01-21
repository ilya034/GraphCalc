using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Entities;

namespace GraphCalc.App;

public static class UserMappingExtensions
{
    public static User ToDomain(this UserDto dto)
        => User.CreateWithId(dto.Id, dto.Username, dto.Email);

    public static User ToDomain(this UserCreateRequest request)
        => User.Create(request.Username, request.Email);

    public static UserDto ToDto(this User domain)
        => new UserDto(domain.Id, domain.Username, domain.Email);

    public static IEnumerable<UserDto> ToDto(this IEnumerable<User> domains)
        => domains.Select(domain => domain.ToDto());
}
