using System.Drawing;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;
using GraphCalc.Presentation.Coordinates;
using GraphCalc.Presentation.Mappers;
using GraphCalc.Presentation.Models;

namespace GraphCalc.Domain.Services;

public class GraphDisplayService : IGraphDisplayService
{
    private readonly ICoordinateTransformer _transformer;

    public GraphDisplayService(ICoordinateTransformer? transformer = null)
    {
        _transformer = transformer ?? new SimpleCoordinateTransformer();
    }

    public RenderableGraph DisplayGraph(Graph graph, double yMin, double yMax, Size screenSize)
    {
        if (graph.Range == null)
            throw new InvalidOperationException("Graph must have a range set before display");

        var xRange = graph.Range;
        var yRange = NumericRange.Create(yMin, yMax, 0.1);

        return GraphToRenderableGraphMapper.Map(
            graph,
            _transformer,
            xRange,
            yRange,
            screenSize);
    }

    public RenderableGraph DisplayGraphWithAutoYRange(Graph graph, Size screenSize)
    {
        if (graph.Range == null)
            throw new InvalidOperationException("Graph must have a range set before display");

        var xRange = graph.Range;
        
        // Calculate Y range from points
        var yValues = graph.Points
            .Where(p => !double.IsNaN(p.Y) && !double.IsInfinity(p.Y))
            .Select(p => p.Y);

        var yMin = yValues.Any() ? yValues.Min() : -1;
        var yMax = yValues.Any() ? yValues.Max() : 1;
        var padding = (yMax - yMin) * 0.1;
        
        var yRange = NumericRange.Create(yMin - padding, yMax + padding, 0.1);

        return GraphToRenderableGraphMapper.Map(
            graph,
            _transformer,
            xRange,
            yRange,
            screenSize);
    }
}