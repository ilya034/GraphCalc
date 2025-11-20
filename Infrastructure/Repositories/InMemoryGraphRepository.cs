using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Infrastructure.Repositories;

public sealed class InMemoryGraphRepository : IGraphRepository
{
    private readonly Dictionary<Guid, Graph> graphs = new();

    public Graph Add(Graph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);

        graphs[graph.Id] = graph;
        return graph;
    }

    public Graph Update(Graph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);

        if (!graphs.ContainsKey(graph.Id))
            throw new KeyNotFoundException($"Graph with id {graph.Id} not found");

        graphs[graph.Id] = graph;
        return graph;
    }

    public Graph? GetById(Guid id)
    {
        return graphs.TryGetValue(id, out var graph) ? graph : null;
    }

    public IEnumerable<Graph> GetByExpression(Guid expressionId)
    {
        return graphs.Values
            .Where(g => g.Expression.Id == expressionId)
            .ToList();
    }

    public IEnumerable<Graph> GetAll()
    {
        return graphs.Values.ToList();
    }

    public bool Remove(Guid id)
    {
        return graphs.Remove(id);
    }

    public void Clear()
    {
        graphs.Clear();
    }

}
