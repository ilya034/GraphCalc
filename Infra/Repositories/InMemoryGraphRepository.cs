using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Infra.Repositories;

internal class InMemoryGraphRepository : InMemoryRepository<Graph>, IGraphRepository
{
    public IEnumerable<Graph> GetByUserId(Guid userId)
    {
        return entities.Values
            .Where(graph => graph.AuthorId == userId)
            .AsEnumerable();
    }
}