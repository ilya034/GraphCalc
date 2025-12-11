using System.Drawing;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Services;
using GraphCalc.Domain.ValueObjects;
using GraphCalc.Presentation.Coordinates;
using GraphCalc.Presentation.Mappers;
using GraphCalc.Presentation.Models;

namespace GraphCalc.Presentation.Services;

public class GraphDisplayService : IGraphDisplayService
{
    private readonly ICoordinateTransformer _transformer;
    private readonly IGraphCalculationService _calculationService;

    public GraphDisplayService(
        ICoordinateTransformer? transformer = null,
        IGraphCalculationService? calculationService = null)
    {
        _transformer = transformer ?? new SimpleCoordinateTransformer();
        _calculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));
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
        var yRange = CalculateYRangeFromGraphWithPadding(graph);

        return GraphToRenderableGraphMapper.Map(
            graph,
            _transformer,
            xRange,
            yRange,
            screenSize);
    }

    private NumericRange CalculateYRangeFromGraphWithPadding(Graph graph, double paddingFactor = 0.1)
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
        var yRange = NumericRange.Create(yMin, yMax, 0.1);

        var padding = (yRange.Max - yRange.Min) * paddingFactor;

        return NumericRange.Create(
            yRange.Min - padding,
            yRange.Max + padding,
            yRange.Step);
    }
}