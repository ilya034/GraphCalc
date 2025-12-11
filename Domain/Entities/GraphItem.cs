using GraphCalc.Domain.Common;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Entities;

public class GraphItem : Entity
{
    private GraphItem(Guid id, MathExpression expression, string independentVariable)
        : base(id)
    {
        Expression = expression;
        IndependentVariable = independentVariable;
    }

    public MathExpression Expression { get; private set; }
    public string IndependentVariable { get; private init; }
    public NumericRange? Range { get; private set; }
    public bool IsVisible { get; private set; } = true;

    internal static GraphItem Create(
        MathExpression expression,
        string independentVariable)
         => new GraphItem(
                Guid.NewGuid(),
                expression,
                independentVariable);

    internal GraphItem WithRange(NumericRange range)
    {
        Range = range;
        return this;
    }

    internal GraphItem WithExpression(MathExpression newExpression)
    {
        Expression = newExpression;
        return this;
    }

    internal GraphItem SetVisibility(bool isVisible)
    {
        IsVisible = isVisible;
        return this;
    }
}