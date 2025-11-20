using GraphCalc.Domain.Interfaces;
using CodingSeb.ExpressionEvaluator;
using GraphCalc.Domain.Entities;

namespace GraphCalc.Infrastructure.ExpressionEvaluation;

/// <summary>
/// Implement IExpressionEvaluator on CodingSeb.ExpressionEvaluator
/// https://github.com/codingseb/ExpressionEvaluator
/// </summary>
public sealed class CodingSebExpressionEvaluator : IExpressionEvaluator
{
    private readonly ExpressionEvaluator evaluator;
    private readonly object locker = new();

    public CodingSebExpressionEvaluator()
    {
        evaluator = new ExpressionEvaluator
        {
            OptionCaseSensitiveEvaluationActive = false,
            OptionForceIntegerNumbersEvaluationsAsDoubleByDefault = true
        };

        ConfigureMathFunctions();
    }

    private void ConfigureMathFunctions()
    {
        evaluator.Variables["pi"] = Math.PI;
        evaluator.Variables["e"] = Math.E;
    }

    public double Evaluate(MathExpression expression, double variableValue)
    {
        lock (locker)
        {
            evaluator.Variables[expression.VariableName] = variableValue;
            var result = evaluator.Evaluate(expression.Text);
            var doubleResult = Convert.ToDouble(result);
            return doubleResult;
        }
    }

    public IEnumerable<double> EvaluateBatch(MathExpression expression, IEnumerable<double> variableValues)
    {
        var results = new List<double>();

        foreach (var value in variableValues)
        {
            var result = Evaluate(expression, value);
            results.Add(result);
        }

        return results;
    }
}
