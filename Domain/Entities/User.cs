using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.Entities;

public class User : Entity
{
    private User(Guid id, string username, string email)
        : base(id)
    {
        Username = username;
        Email = email;
    }

    public string Username { get; private set; }
    public string Email { get; private set; }

    public static User Create(string username, string email, string? description = null)
    {
        return new User(
            Guid.NewGuid(),
            username,
            email);
    }
}
