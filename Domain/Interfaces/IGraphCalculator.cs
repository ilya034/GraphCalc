using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Interfaces;

public interface IGraphCalculator
{
    IEnumerable<MathPoint> Calculate(string graphItem, NumericRange range);
}
