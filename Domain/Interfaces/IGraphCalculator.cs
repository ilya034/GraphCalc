using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Interfaces;

public interface IGraphCalculator
{
    IEnumerable<MathPoint> Calculate(Graph graph);
    IEnumerable<MathPoint> FindRoots(Graph graph, double precision = 0.001);
    IEnumerable<MathPoint> FindExtrema(Graph graph, double precision = 0.001);
}
