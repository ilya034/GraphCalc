using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.Entities;

public class GraphSet : Entity
{
    private readonly List<Graph> graphs = new();

    private GraphSet(Guid id, Guid authorId)
        : base(id)
    {
        AuthorId = authorId;
    }

    public Guid AuthorId { get; private set; }
    public IReadOnlyList<Graph> Graphs => graphs.AsReadOnly();

    public static GraphSet Create(Guid authorId, IEnumerable<Graph> graphs)
    {
        var set = new GraphSet(Guid.NewGuid(), authorId);

        foreach (var graph in graphs)
            set.AddGraph(graph);

        return set;
    }

    public void AddGraph(Graph graph)
    {
        graphs.Add(graph);
    }

    public void RemoveGraph(Guid graphId)
    {
        if (graphs.Count <= 1 && graphs.Any(g => g.Id == graphId))
            throw new InvalidOperationException("Cannot remove the last graph from the set");

        graphs.RemoveAll(g => g.Id == graphId);
    }
}