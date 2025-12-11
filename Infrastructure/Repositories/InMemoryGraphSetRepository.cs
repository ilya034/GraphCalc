using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Infrastructure.Repositories;

public class InMemoryGraphSetRepository : IGraphSetRepository
{
    private readonly Dictionary<Guid, GraphSet> _store = new();

    public GraphSet? GetById(Guid id)
    {
        _store.TryGetValue(id, out var graphSet);
        return graphSet;
    }

    public IEnumerable<GraphSet> GetByUserId(Guid userId)
    {
        return _store.Values.Where(gs => gs.UserId == userId);
    }

    public void Add(GraphSet graphSet)
    {
        _store[graphSet.Id] = graphSet;
    }

    public void Update(GraphSet graphSet)
    {
        if (_store.ContainsKey(graphSet.Id))
            _store[graphSet.Id] = graphSet;
    }

    public void Remove(Guid id)
    {
        _store.Remove(id);
    }
}
