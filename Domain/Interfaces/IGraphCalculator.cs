using System.Drawing;
using GraphCalc.Domain.Entities;

namespace GraphCalc.Domain.Interfaces;

public interface IGraphCalculator
{
    IEnumerable<Point> Calculate(Graph graph);
    IEnumerable<Point> FindRoots(Graph graph, double precision = 0.001);
    IEnumerable<Point> FindExtrema(Graph graph, double precision = 0.001);
}
