using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Domain.Services;

internal class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly IGraphRepository graphRepository;

    public UserService(IUserRepository userRepository, IGraphRepository graphRepository)
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

    public User CreateUser(User user)
    {
        userRepository.Add(user);
        return user;
    }

    public User UpdateUser(Guid id, User user)
    {
        var updatedUser = User.CreateWithId(id, user.Username, user.Email);
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
