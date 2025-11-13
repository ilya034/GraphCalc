using GraphCalc.Domain.Common;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Entities;

public class Graph : Entity
{
    private readonly List<MathPoint> points = new();

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
    public NumericRange? Range { get; private set; }
    public bool IsVisible { get; private set; }
    public IReadOnlyList<MathPoint> Points => points.AsReadOnly();

    public static Graph Create(
        MathExpression expression,
        string independentVariable,
        GraphStyle? style = null)
         => new Graph(
                Guid.NewGuid(),
                expression,
                independentVariable,
                style ?? GraphStyle.Default);

    public Graph WithRange(NumericRange range)
    {
        Range = range;
        return this;
    }

    public Graph WithStyle(GraphStyle style)
    {
        Style = style;
        return this;
    }

    public Graph SetPoints(IEnumerable<MathPoint> points)
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