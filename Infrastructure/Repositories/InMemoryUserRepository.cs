using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Infrastructure.Persistence;

namespace GraphCalc.Infrastructure.Repositories;

public class InMemoryUserRepository : InMemoryRepository<User>, IUserRepository
{
    public User? GetByEmail(string email) 
        => GetAll().FirstOrDefault(u => u.Email == email);

    public User? GetByUsername(string username) 
        => GetAll().FirstOrDefault(u => u.Username == username);
}
