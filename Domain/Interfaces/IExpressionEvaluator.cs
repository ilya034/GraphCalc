using GraphCalc.Domain.Entities;

namespace GraphCalc.Domain.Interfaces;

public interface IExpressionEvaluator
{
    double Evaluate(MathExpression expression, double variableValue);
    IEnumerable<double> EvaluateBatch(MathExpression expression, IEnumerable<double> variableValues);
}
