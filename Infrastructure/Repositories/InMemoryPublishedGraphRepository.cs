using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Infrastructure.Persistence;

namespace GraphCalc.Infrastructure.Repositories;

public class InMemoryPublishedGraphRepository : InMemoryRepository<PublishedGraph>
{
    public IReadOnlyList<PublishedGraph> GetByUserId(Guid userId)
        => GetAll()
            .Where(pg => pg.UserId == userId && pg.IsActive)
            .ToList()
            .AsReadOnly();

    public IReadOnlyList<PublishedGraph> GetByGraphId(Guid graphId)
        => GetAll()
            .Where(pg => pg.GraphId == graphId && pg.IsActive)
            .ToList()
            .AsReadOnly();
}
