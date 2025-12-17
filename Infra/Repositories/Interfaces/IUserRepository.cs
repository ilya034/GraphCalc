using GraphCalc.Domain.Entities;

namespace GraphCalc.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    User GetByEmail(string email);
}
