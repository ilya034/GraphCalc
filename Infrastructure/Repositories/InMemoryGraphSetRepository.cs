using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Infrastructure.Persistence;

namespace GraphCalc.Infrastructure.Repositories;

public class InMemoryGraphSetRepository : InMemoryRepository<GraphSet>, IGraphSetRepository
{
    public IEnumerable<GraphSet> GetByAuthorId(Guid authorId)
    {
        return store.Values.Where(gs => gs.AuthorId == authorId);
    }
}
