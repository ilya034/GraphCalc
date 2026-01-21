using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Infra.Repositories;

internal class InMemoryUserRepository : InMemoryRepository<User>, IUserRepository
{
    public User GetByEmail(string email)
    {
        return entities.Values
            .FirstOrDefault(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            ?? throw new KeyNotFoundException($"User with email {email} not found.");
    }
}