using System.Drawing;
using GraphCalc.Domain.Common;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Entities;

public class Graph : Entity
{
    private readonly List<Point> points = new();

    private Graph(Guid id, MathExpression expression, string independentVariable, GraphStyle style)
        : base(id)
    {
        Expression = expression;
        IndependentVariable = independentVariable;
        Style = style;
        IsVisible = true;
    }

    public MathExpression Expression { get; private set; }
    public string IndependentVariable { get; private init; }
    public GraphStyle Style { get; private set; }
    public Range? Range { get; private set; }
    public bool IsVisible { get; private set; }
    public IReadOnlyList<Point> Points => points.AsReadOnly();

    public static Graph Create(
        MathExpression expression,
        string independentVariable,
        GraphStyle? style = null)
         => new Graph(
                Guid.NewGuid(),
                expression,
                independentVariable,
                style ?? GraphStyle.Default);

    public Graph WithRange(Range range)
    {
        Range = range;
        return this;
    }

    public Graph WithStyle(GraphStyle style)
    {
        Style = style;
        return this;
    }

    public Graph Show()
    {
        IsVisible = true;
        return this;
    }

    public Graph Hide()
    {
        IsVisible = false;
        return this;
    }

    public Graph ToggleVisibility()
    {
        IsVisible = !IsVisible;
        return this;
    }

    public Graph SetPoints(IEnumerable<Point> points)
    {
        this.points.Clear();
        this.points.AddRange(points);
        return this;
    }

    public Graph ClearPoints()
    {
        points.Clear();
        return this;
    }

    public Graph WithExpression(MathExpression newExpression)
    {
        Expression = newExpression;
        points.Clear();
        return this;
    }
}