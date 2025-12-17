using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.App;

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
        try
        {
            var user = userRepository.GetById(id);
            return user.ToDto();
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
    }

    public UserDto CreateUser(UserDto userDto)
    {
        var user = userDto.ToDomain();
        userRepository.Add(user);
        return user.ToDto();
    }

    public UserDto UpdateUser(Guid id, UserDto userDto)
    {
        try
        {
            var existingUser = userRepository.GetById(id);
            var updatedUser = User.CreateWithId(id, userDto.Username, userDto.Email);
            userRepository.Update(updatedUser);
            return updatedUser.ToDto();
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
    }

    public void DeleteUser(Guid id)
    {
        try
        {
            userRepository.Delete(id);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
    }

    public IEnumerable<GraphDto> GetGraphsByUserId(Guid userId)
    {
        var graphs = graphRepository.GetByUserId(userId);
        return graphs.ToDto();
    }
}
