using GraphCalc.Api.Dtos;
using GraphCalc.App;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.Exceptions;

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
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(id));

        try
        {
            var user = userRepository.GetById(id);
            return user;
        }
        catch (KeyNotFoundException)
        {
            throw new EntityNotFoundException(nameof(User), id);
        }
    }

    public User CreateUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        userRepository.Add(user);
        return user;
    }

    public User UpdateUser(Guid id, User user)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(id));

        ArgumentNullException.ThrowIfNull(user);

        // Проверяем, что пользователь существует
        try
        {
            userRepository.GetById(id);
        }
        catch (KeyNotFoundException)
        {
            throw new EntityNotFoundException(nameof(User), id);
        }

        var updatedUser = User.CreateWithId(id, user.Username, user.Email);
        userRepository.Update(updatedUser);
        return updatedUser;
    }

    public void DeleteUser(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(id));

        try
        {
            userRepository.Delete(id);
        }
        catch (KeyNotFoundException)
        {
            throw new EntityNotFoundException(nameof(User), id);
        }
    }

    public IEnumerable<Graph> GetGraphsByUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        var graphs = graphRepository.GetByUserId(userId);
        return graphs;
    }
}
