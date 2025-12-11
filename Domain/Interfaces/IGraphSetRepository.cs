using GraphCalc.Domain.Entities;

namespace GraphCalc.Domain.Interfaces;

public interface IGraphSetRepository
{
    GraphSet? GetById(Guid id);
    IEnumerable<GraphSet> GetByUserId(Guid userId);
    void Add(GraphSet graphSet);
    void Update(GraphSet graphSet);
    void Remove(Guid id);
}