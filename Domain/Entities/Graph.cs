using GraphCalc.Domain.Common;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Entities;

public class Graph : Entity
{
    private Graph(Guid id, NumericRange range, Guid authorId, List<GraphItem> items)
        : base(id)
    {
        Range = range;
        AuthorId = authorId;
        Items = items;
    }

    public NumericRange Range { get; private set; }
    public Guid AuthorId { get; private set; }
    public List<GraphItem> Items { get; private set; } = new();

    public static Graph Create(NumericRange range, Guid authorId, List<GraphItem>? items = null)
    {
        return new Graph(
            Guid.NewGuid(),
            range,
            authorId,
            items ?? new List<GraphItem>());
    }

    public void AddItem(GraphItem item)
    {
        Items.Add(item);
    }

    public void RemoveItem(GraphItem item)
    {
        Items.Remove(item);
    }

    public void UpdateRange(NumericRange newRange)
    {
        Range = newRange;
    }
}