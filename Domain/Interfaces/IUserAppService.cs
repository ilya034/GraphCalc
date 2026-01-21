using GraphCalc.Domain.Entities;
using GraphCalc.Api.Dtos;

namespace GraphCalc.Domain.Services;

public interface IUserAppService
{
    IEnumerable<User> GetAllUsers();
    User GetUserById(Guid id);
    User CreateUser(UserCreateRequest request);
    User UpdateUser(Guid id, UserDto userDto);
    void DeleteUser(Guid id);
    IEnumerable<Graph> GetGraphsByUserId(Guid userId);
}
