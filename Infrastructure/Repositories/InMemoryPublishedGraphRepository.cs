using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Infrastructure.Repositories;

public class InMemoryPublishedGraphRepository : InMemoryRepository<PublishedGraph>
{
    public IReadOnlyList<PublishedGraph> GetByUserId(Guid userId)
        => items.Values
            .Where(pg => pg.UserId == userId && pg.IsActive)
            .ToList()
            .AsReadOnly();

    public IReadOnlyList<PublishedGraph> GetByGraphId(Guid graphId)
        => items.Values
            .Where(pg => pg.GraphId == graphId && pg.IsActive)
            .ToList()
            .AsReadOnly();
}
