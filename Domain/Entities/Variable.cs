using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.Entities;

public class Variable : Entity
{
    private Variable(Guid id, string name, double defaultValue = 0)
        : base(id)
    {
        Name = name;
        DefaultValue = defaultValue;
        CurrentValue = defaultValue;
    }

    public string Name { get; private init; }
    public double DefaultValue { get; private set; }
    public double CurrentValue { get; private set; }

    public static Variable Create(string name, double defaultValue = 0)
        => new Variable(Guid.NewGuid(), name, defaultValue);

    public Variable SetValue(double value)
    {
        if (!double.IsNaN(value) && !double.IsInfinity(value))
            CurrentValue = value;
        return this;
    }

    public Variable SetDefaultValue(double value)
    {
        if (!double.IsNaN(value) && !double.IsInfinity(value))
            DefaultValue = value;
        return this;
    }

    public Variable Reset()
    {
        CurrentValue = DefaultValue;
        return this;
    }
}