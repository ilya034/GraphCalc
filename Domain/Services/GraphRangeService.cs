using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Services;

public class GraphRangeService : IGraphRangeService
{
    public NumericRange CalculateYRangeFromGraph(Graph graph)
    {
        if (graph.Points == null || !graph.Points.Any())
            return NumericRange.Create(-1, 1, 0.1);

        var yValues = graph.Points
            .Where(p => !double.IsNaN(p.Y) && !double.IsInfinity(p.Y))
            .Select(p => p.Y);

        if (!yValues.Any())
            return NumericRange.Create(-1, 1, 0.1);

        var yMin = yValues.Min();
        var yMax = yValues.Max();

        return NumericRange.Create(yMin, yMax, 0.1);
    }

    public NumericRange CalculateYRangeFromGraphWithPadding(Graph graph, double paddingFactor = 0.1)
    {
        var yRange = CalculateYRangeFromGraph(graph);
        var padding = (yRange.Max - yRange.Min) * paddingFactor;

        return NumericRange.Create(
            yRange.Min - padding,
            yRange.Max + padding,
            yRange.Step);
    }
}