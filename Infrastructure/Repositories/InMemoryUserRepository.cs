using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Infrastructure.Repositories;

public class InMemoryUserRepository : InMemoryRepository<User>, IUserRepository
{
    public User? GetByEmail(string email) 
        => items.Values.FirstOrDefault(u => u.Email == email);

    public User? GetByUsername(string username) 
        => items.Values.FirstOrDefault(u => u.Username == username);
}
