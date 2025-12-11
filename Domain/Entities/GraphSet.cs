using GraphCalc.Domain.Common;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Entities;

public class GraphSet : Entity
{
    private readonly List<Graph> graphs = new();

    private GraphSet(Guid id, Guid authorId) : base(id) 
    {
        AuthorId = authorId;
    }

    public IReadOnlyList<Graph> Graphs => graphs.AsReadOnly();
    public NumericRange? Range { get; private set; }
    public Guid AuthorId { get; private set; }

    public static GraphSet Create(Guid authorId) => new(Guid.NewGuid(), authorId);

    public GraphSet WithRange(NumericRange range)
    {
        Range = range;
        return this;
    }

    public GraphSet AddGraph(Graph graph)
    {
        if (graphs.Any(g => g.Id == graph.Id))
            return this;

        graphs.Add(graph);
        if (Range != null)
            graph.WithRange(Range);

        return this;
    }

    public GraphSet RemoveGraph(Guid graphId)
    {
        if (graphs.Count <= 1)
            throw new InvalidOperationException("GraphSet must contain at least one graph");

        graphs.RemoveAll(g => g.Id == graphId);
        return this;
    }

    public GraphSet ApplyRange()
    {
        if (Range == null)
            return this;

        foreach (var graph in graphs)
            graph.WithRange(Range);

        return this;
    }
}