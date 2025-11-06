using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.Expressions;

public class Expression : Entity
{
    public string Text { get; private set; }
    public IExpressionNode RootNode { get; private set; }
    
    public Expression(Guid id) : base(id) { }

    public Expression(Guid id, string text, IExpressionNode rootNode) : base(id)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException(nameof(text));

        Text = text;
        RootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
    }
}