using GraphCalc.Domain.Expressions;
using GraphCalc.Domain.Services;
using GraphCalc.Domain.Variables;

public class Program
{
    public static void Main(string[] args)
    {
        var ast = new BinaryOperationNode(
            Left: new FunctionCallNode(
                FunctionName: "max",
                Arguments: new List<IExpressionNode> { new VariableNode("y"), new NumericLiteralNode(4) }
            ),
            Right: new FunctionCallNode(
                FunctionName: "sin",
                Arguments: new List<IExpressionNode> { new VariableNode("x") }
            ),
            Operator: OperatorType.Multiply
        );
        var expression = new Expression(Guid.NewGuid(), "max(y, 4) * sin(x)", ast);

        // Теперь мы создаем список объектов Variable, а не просто словарь.
        // Это делает код более явным и доменно-ориентированным.
        var variables = new List<Variable>
        {
            new Variable("x", Math.PI / 2),
            new Variable("y", 3)
        };
        Console.WriteLine($"Вычисляем для x = {variables[0].Value}, y = {variables[1].Value}");

        var evaluator = new ExpressionEvaluator(variables);

        // Вычисляем результат. Ожидаем: max(3, 4) * sin(PI/2) = 4 * 1 = 4
        double result = expression.RootNode.Accept(evaluator);
        Console.WriteLine($"Результат вычисления: {result}");
    }
}