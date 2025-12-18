using GraphCalc.Domain.Interfaces;
using CodingSeb.ExpressionEvaluator;

namespace GraphCalc.Infra.ExpressionEvaluation;

/// <summary>
/// Implement IExpressionEvaluator using CodingSeb.ExpressionEvaluator
/// https://github.com/codingseb/ExpressionEvaluator
/// </summary>
public sealed class CodingSebExpressionEvaluator : IExpressionEvaluator
{
    private readonly ExpressionEvaluator evaluator;
    private readonly object locker = new();
    private const string DefaultVariableName = "x";

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

    public double Evaluate(string expression, double variableValue)
    {
        lock (locker)
        {
            evaluator.Variables[DefaultVariableName] = variableValue;
            
            try
            {
                var result = evaluator.Evaluate(expression);
                var doubleResult = Convert.ToDouble(result);
                return doubleResult;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to evaluate expression: '{expression}'", ex);
            }
        }
    }

    public IEnumerable<double> EvaluateBatch(string expression, IEnumerable<double> variableValues)
    {
        foreach (var value in variableValues)
            yield return Evaluate(expression, value);
    }
}
