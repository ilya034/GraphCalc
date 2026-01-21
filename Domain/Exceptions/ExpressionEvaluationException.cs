namespace GraphCalc.Domain.Exceptions;

/// <summary>
/// Исключение при ошибке вычисления выражения
/// </summary>
public class ExpressionEvaluationException : DomainException
{
    public string Expression { get; set; }
    public double VariableValue { get; set; }

    public ExpressionEvaluationException(string expression, double variableValue, Exception innerException)
        : base($"Failed to evaluate expression '{expression}' with x={variableValue}.", "EXPRESSION_EVALUATION_ERROR")
    {
        Expression = expression;
        VariableValue = variableValue;
    }
}
