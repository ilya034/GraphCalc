using GraphCalc.Api.Dtos;

namespace GraphCalc.Domain.Services;

public interface IUserAppService
{
    IEnumerable<UserDto> GetAllUsers();
    UserDto GetUserById(Guid id);
    UserDto CreateUser(UserCreateRequest request);
    UserDto UpdateUser(Guid id, UserDto userDto);
    void DeleteUser(Guid id);
    IEnumerable<GraphDto> GetGraphsByUserId(Guid userId);
}
