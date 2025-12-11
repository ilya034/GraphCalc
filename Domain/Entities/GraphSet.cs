using GraphCalc.Domain.Common;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Entities;

public class GraphSet : Entity
{
    private readonly List<GraphItem> items = new();
    private Guid userId;

    private GraphSet(Guid id, Guid userId) : base(id)
    {
        this.userId = userId;
    }

    public Guid UserId => userId;
    public IReadOnlyList<GraphItem> Items => items.AsReadOnly();
    public NumericRange? GlobalRange { get; private set; }

    public static GraphSet Create(Guid userId) => new(Guid.NewGuid(), userId);

    public GraphSet WithGlobalRange(NumericRange range)
    {
        GlobalRange = range;
        return this;
    }

    public GraphSet AddGraph(string expression, string independentVariable)
    {
        var graphItem = GraphItem.Create(
            MathExpression.Create(expression),
            independentVariable);

        if (GlobalRange != null)
            graphItem.WithRange(GlobalRange);

        items.Add(graphItem);
        return this;
    }

    public GraphSet UpdateGraphExpression(Guid graphItemId, string newExpression)
    {
        var item = items.FirstOrDefault(g => g.Id == graphItemId);
        if (item != null)
            item.WithExpression(MathExpression.Create(newExpression));

        return this;
    }

    public GraphSet RemoveGraph(Guid graphItemId)
    {
        items.RemoveAll(g => g.Id == graphItemId);
        return this;
    }

    public GraphSet SetGraphVisibility(Guid graphItemId, bool isVisible)
    {
        var item = items.FirstOrDefault(g => g.Id == graphItemId);
        if (item != null)
            item.SetVisibility(isVisible);

        return this;
    }
}