using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.Entities;

public class User : Entity
{
    private readonly List<Guid> publishedGraphIds = new();

    private User(Guid id, string username, string email, string? description = null)
        : base(id)
    {
        Username = username;
        Email = email;
        Description = description;
    }

    public string Username { get; private set; }
    public string Email { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyList<Guid> PublishedGraphIds => publishedGraphIds.AsReadOnly();

    public static User Create(string username, string email, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username не может быть пустым", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email не может быть пустым", nameof(email));

        return new User(
            Guid.NewGuid(),
            username,
            email,
            description);
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void PublishGraph(Guid graphId)
    {
        if (!publishedGraphIds.Contains(graphId))
            publishedGraphIds.Add(graphId);
    }

    public void UnpublishGraph(Guid graphId) => publishedGraphIds.Remove(graphId);

    public void ClearPublishedGraphs() => publishedGraphIds.Clear();
}
