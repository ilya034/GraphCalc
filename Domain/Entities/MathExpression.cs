using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.Entities;

public class MathExpression : Entity
{
    private readonly List<Variable> variables = new();

    private MathExpression(Guid id, string text, string? name = null)
        : base(id)
    {
        Text = text;
        Name = name;
    }

    public string Text { get; private set; }
    public string VariableName => variables.FirstOrDefault()?.Name ?? "x";
    public string? Name { get; private set; }
    public IReadOnlyList<Variable> Variables => variables.AsReadOnly();

    public static MathExpression Create(string text, string? name = null)
        => new MathExpression(Guid.NewGuid(), text, name);

    public MathExpression WithText(string newText)
    {
        Text = newText;
        variables.Clear();
        return this;
    }

    public MathExpression WithName(string? name)
    {
        Name = name;
        return this;
    }

    public MathExpression AddVariable(Variable variable)
    {
        if (!variables.Any(v => v.Name == variable.Name))
            variables.Add(variable);
        return this;
    }

    public MathExpression RemoveVariable(string name)
    {
        variables.RemoveAll(v => v.Name == name);
        return this;
    }

    public MathExpression ClearVariables()
    {
        variables.Clear();
        return this;
    }

    public Variable GetVariable(string name)
    {
        var variable = variables.FirstOrDefault(v => v.Name == name);

        if (variable == null)
            throw new KeyNotFoundException($"Variable '{name}' not found");

        return variable;
    }
}