using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Interfaces;

public interface IGraphCalculator
{
    IEnumerable<MathPoint> Calculate(Graph graph, NumericRange range);
}