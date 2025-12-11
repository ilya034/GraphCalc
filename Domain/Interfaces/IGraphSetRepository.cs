using GraphCalc.Domain.Entities;

namespace GraphCalc.Domain.Interfaces;

public interface IGraphSetRepository : IRepository<Graph>
{
    IEnumerable<GraphSet> GetByUserId(Guid userId);
}