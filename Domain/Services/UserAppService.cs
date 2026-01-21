using GraphCalc.Api.Dtos;
using GraphCalc.App;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Domain.Services;

internal class UserAppService : IUserAppService
{
    private readonly IUserRepository userRepository;
    private readonly IGraphRepository graphRepository;

    public UserAppService(IUserRepository userRepository, IGraphRepository graphRepository)
    {
        this.userRepository = userRepository;
        this.graphRepository = graphRepository;
    }

    public IEnumerable<User> GetAllUsers()
    {
        var users = userRepository.GetAll();
        return users;
    }

    public User GetUserById(Guid id)
    {
        var user = userRepository.GetById(id);
        return user;
    }

    public User CreateUser(UserCreateRequest request)
    {
        var user = request.ToDomain();
        userRepository.Add(user);
        return user;
    }

    public User UpdateUser(Guid id, UserDto userDto)
    {
        var existingUser = userRepository.GetById(id);
        var updatedUser = User.CreateWithId(id, userDto.Username, userDto.Email);
        userRepository.Update(updatedUser);
        return updatedUser;
    }

    public void DeleteUser(Guid id)
    {
        userRepository.Delete(id);
    }

    public IEnumerable<Graph> GetGraphsByUserId(Guid userId)
    {
        var graphs = graphRepository.GetByUserId(userId);
        return graphs;
    }
}
