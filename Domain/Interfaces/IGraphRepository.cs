using GraphCalc.Domain.Entities;

namespace GraphCalc.Domain.Interfaces;

public interface IGraphRepository
{
    Graph Add(Graph graph);
    Graph Update(Graph graph);
    Graph? GetById(Guid id);
    IEnumerable<Graph> GetByExpression(Guid expressionId);
    IEnumerable<Graph> GetAll();
    bool Remove(Guid id);
    void Clear();
}
