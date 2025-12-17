using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Infrastructure.GraphCalculation;

public sealed class SimpleGraphCalculator : IGraphCalculator
{
    private readonly IExpressionEvaluator evaluator;

    public SimpleGraphCalculator(IExpressionEvaluator evaluator)
    {
        this.evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
    }

    public IEnumerable<MathPoint> Calculate(string Expression, NumericRange range)
    {
        for (double x = range.Min; x <= range.Max; x += range.Step)
        {
            MathPoint? point = null;
            try
            {
                double y = evaluator.Evaluate(Expression, x);

                if (!double.IsInfinity(y) && !double.IsNaN(y))
                    point = new MathPoint(x, y);
            }
            catch (Exception)
            {
            }

            if (point != null)
                yield return point;
        }
    }
}
