namespace GraphCalc.Domain.Interfaces;

public interface IExpressionEvaluator
{
    double Evaluate(String expression, double variableValue);
    IEnumerable<double> EvaluateBatch(string expression, IEnumerable<double> variableValues);
}
