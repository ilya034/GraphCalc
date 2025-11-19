using GraphCalc.Domain.Common;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Entities;

public class Graph : Entity
{
    private readonly List<MathPoint> points = new();

    private Graph(Guid id, MathExpression expression, string independentVariable)
        : base(id)
    {
        Expression = expression;
        IndependentVariable = independentVariable;
    }

    public MathExpression Expression { get; private set; }
    public string IndependentVariable { get; private init; }
    public NumericRange? Range { get; private set; }
    public IReadOnlyList<MathPoint> Points => points.AsReadOnly();

    public static Graph Create(
        MathExpression expression,
        string independentVariable)
         => new Graph(
                Guid.NewGuid(),
                expression,
                independentVariable);

    public Graph WithRange(NumericRange range)
    {
        Range = range;
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