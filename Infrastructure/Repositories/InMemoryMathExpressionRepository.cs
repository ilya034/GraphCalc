using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Infrastructure.Persistence;

public class UserInMemoryRepository : InMemoryRepository<User>, IUserRepository
{
    public User? GetByEmail(string email)
    {
        var user = store.Values.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return user;
    }

    public User? GetByUsername(string username)
    {
        var user = store.Values.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return user;
    }
}