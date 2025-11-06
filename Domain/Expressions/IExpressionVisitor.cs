namespace GraphCalc.Domain.Expressions;

public interface IExpressionNodeVisitor<T>
{
    T Visit(NumericLiteralNode node);
    T Visit(VariableNode node);
    T Visit(BinaryOperationNode node);
    T Visit(FunctionCallNode node);
}
