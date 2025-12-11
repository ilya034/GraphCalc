using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Infrastructure.GraphCalculation;

public sealed class NumericalGraphCalculator : IGraphCalculator
{
    private readonly IExpressionEvaluator evaluator;


    public NumericalGraphCalculator(IExpressionEvaluator evaluator)
    {
        this.evaluator = evaluator;
    }

    public IEnumerable<MathPoint> Calculate(Graph graph, NumericRange range)
    {
        var mathExpr = MathExpression.Create(graph.Expression);

        foreach (var x in range.GetValues())
        {
            double y;
            try
            {
                y = evaluator.Evaluate(mathExpr, x);
            }
            catch
            {
                y = double.NaN;
            }

            if (!double.IsNaN(y) && !double.IsInfinity(y))
                yield return new MathPoint(x, y);
        }
    }
}