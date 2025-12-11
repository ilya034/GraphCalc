using GraphCalc.Domain.Common;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Entities;

public class PublishedGraph : Entity
{
    private PublishedGraph(Guid id, Guid userId, Guid graphId, PublicationMetadata metadata)
        : base(id)
    {
        UserId = userId;
        GraphId = graphId;
        Metadata = metadata;
        IsActive = true;
    }

    public Guid UserId { get; private set; }
    public Guid GraphId { get; private set; }
    public PublicationMetadata Metadata { get; private set; }
    public bool IsActive { get; private set; }

    public static PublishedGraph Create(
        Guid userId,
        Guid graphId,
        string title,
        string? description = null,
        bool isPublic = true)
    {
        return new PublishedGraph(
            Guid.NewGuid(),
            userId,
            graphId,
            PublicationMetadata.Create(title, description, isPublic));
    }

    public void UpdateMetadata(PublicationMetadata newMetadata)
    {
        Metadata = newMetadata ?? throw new ArgumentNullException(nameof(newMetadata));
    }
}
