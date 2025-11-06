using GraphCalc.Domain.Expressions;
using GraphCalc.Domain.Variables;

namespace GraphCalc.Domain.Services;

public class ExpressionEvaluator : IExpressionNodeVisitor<double>
{
    private readonly IReadOnlyDictionary<string, double> variableValues;

    private static readonly IReadOnlyDictionary<OperatorType, Func<double, double, double>> BinaryOperations =
        new Dictionary<OperatorType, Func<double, double, double>>
        {
            [OperatorType.Add] = (a, b) => a + b,
            [OperatorType.Subtract] = (a, b) => a - b,
            [OperatorType.Multiply] = (a, b) => a * b,
            [OperatorType.Divide] = (a, b) => b == 0 ? double.NaN : a / b,
            [OperatorType.Power] = Math.Pow
        };

    private static readonly IReadOnlyDictionary<string, Func<IReadOnlyList<double>, double>> Functions =
        new Dictionary<string, Func<IReadOnlyList<double>, double>>(StringComparer.OrdinalIgnoreCase)
        {
            ["sin"] = args =>
            {
                if (args.Count != 1) throw new InvalidOperationException();
                return Math.Sin(args[0]);
            },
            ["cos"] = args =>
            {
                if (args.Count != 1) throw new InvalidOperationException();
                return Math.Cos(args[0]);
            },
            ["sqrt"] = args =>
            {
                if (args.Count != 1) throw new InvalidOperationException();
                return Math.Sqrt(args[0]);
            },
            ["max"] = args =>
            {
                if (args.Count == 0) throw new InvalidOperationException();
                return args.Max();
            }
        };


    public ExpressionEvaluator(IEnumerable<Variable> variables)
    {
        variableValues = variables.ToDictionary(v => v.Name, v => v.Value);
    }

    public double Visit(NumericLiteralNode node) => node.Value;

    public double Visit(VariableNode node)
    {
        if (variableValues.TryGetValue(node.Name, out double value))
            return value;

        throw new InvalidOperationException($"Variable '{node.Name}' not ");
    }

    public double Visit(BinaryOperationNode node)
    {
        if (BinaryOperations.TryGetValue(node.Operator, out var operation))
        {
            double left = node.Left.Accept(this);
            double right = node.Right.Accept(this);
            return operation(left, right);
        }

        throw new NotSupportedException($"Operator '{node.Operator}' not supported");
    }

    public double Visit(FunctionCallNode node)
    {
        if (Functions.TryGetValue(node.FunctionName, out var function))
        {
            var evaluatedArgs = node.Arguments.Select(arg => arg.Accept(this)).ToList();
            return function(evaluatedArgs);
        }

        throw new NotSupportedException($"Func '{node.FunctionName}' not supported");
    }
}
