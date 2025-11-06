using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.Variables;

public record Variable(string Name, double Value) : ValueObject;