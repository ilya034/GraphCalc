using GraphCalc.Domain.Entities;

namespace GraphCalc.Domain.Interfaces;

public interface IGraphSetRepository : IRepository<GraphSet>
{
    IEnumerable<GraphSet> GetByAuthorId(Guid authorId);
}