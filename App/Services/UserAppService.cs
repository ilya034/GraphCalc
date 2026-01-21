using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.App.Services;

public class UserAppService
{
    private readonly IUserRepository userRepository;
    private readonly IGraphRepository graphRepository;

    public UserAppService(IUserRepository userRepository, IGraphRepository graphRepository)
    {
        this.userRepository = userRepository;
        this.graphRepository = graphRepository;
    }

    public IEnumerable<UserDto> GetAllUsers()
    {
        var users = userRepository.GetAll();
        return users.ToDto();
    }

    public UserDto GetUserById(Guid id)
    {
        var user = userRepository.GetById(id);
        return user.ToDto();
    }

    public UserDto CreateUser(UserCreateRequest request)
    {
        var user = request.ToDomain();
        userRepository.Add(user);
        return user.ToDto();
    }

    public UserDto UpdateUser(Guid id, UserDto userDto)
    {
        var existingUser = userRepository.GetById(id);
        var updatedUser = User.CreateWithId(id, userDto.Username, userDto.Email);
        userRepository.Update(updatedUser);
        return updatedUser.ToDto();
    }

    public void DeleteUser(Guid id)
    {
        userRepository.Delete(id);
    }

    public IEnumerable<GraphDto> GetGraphsByUserId(Guid userId)
    {
        var graphs = graphRepository.GetByUserId(userId);
        return graphs.ToDto();
    }
}
