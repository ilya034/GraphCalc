using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.ValueObjects;

public record PublicationMetadata : ValueObject
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public bool IsPublic { get; init; }

    public PublicationMetadata() { }

    public PublicationMetadata(
        string title,
        string? description = null,
        bool isPublic = true)
    {
        Title = title;
        Description = description;
        IsPublic = isPublic;
    }

    public static PublicationMetadata Create(
        string title,
        string? description = null,
        bool isPublic = true)
    {
        return new()
        {
            Title = title,
            Description = description,
            IsPublic = isPublic,
        };
    }
}
