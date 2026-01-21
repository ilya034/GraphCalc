using GraphCalc.Domain.Entities;

namespace GraphCalc.Domain.Services;

public interface IUserService
{
    IEnumerable<User> GetAllUsers();
    User GetUserById(Guid id);
    User CreateUser(User user);
    User UpdateUser(Guid id, User user);
    void DeleteUser(Guid id);
    IEnumerable<Graph> GetGraphsByUserId(Guid userId);
}
