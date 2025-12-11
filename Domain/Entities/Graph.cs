using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.Entities;

public class Graph : Entity
{
    private Graph(Guid id, string expression, string independentVariable = "x")
    : base(id)
    {
        Expression = expression;
        IndependentVariable = independentVariable;
    }

    public string Expression { get; private set; }
    public string IndependentVariable { get; private set; }

    public static Graph Create(string expression, string independentVariable = "x")
        => new Graph(Guid.NewGuid(), expression, independentVariable);

    public void UpdateExpression(string newExpression)
    {
        Expression = newExpression;
    }
}