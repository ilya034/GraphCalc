namespace GraphCalc.Domain.Expressions;

public interface IExpressionNode
{
    T Accept<T>(IExpressionNodeVisitor<T> visitor);
}

public record NumericLiteralNode(double Value) : IExpressionNode
{
    public T Accept<T>(IExpressionNodeVisitor<T> visitor) => visitor.Visit(this);
}

public record VariableNode(string Name) : IExpressionNode
{
    public T Accept<T>(IExpressionNodeVisitor<T> visitor) => visitor.Visit(this);
}

public enum OperatorType { Add, Subtract, Multiply, Divide, Power }

public record BinaryOperationNode(
    IExpressionNode Left,
    IExpressionNode Right,
    OperatorType Operator) : IExpressionNode
{
    public T Accept<T>(IExpressionNodeVisitor<T> visitor) => visitor.Visit(this);
}

public record FunctionCallNode(
    string FunctionName,
    IReadOnlyList<IExpressionNode> Arguments) : IExpressionNode
{
    public T Accept<T>(IExpressionNodeVisitor<T> visitor) => visitor.Visit(this);
}
