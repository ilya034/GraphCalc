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
        var yRange = _calculationService.CalculateYRangeFromGraphWithPadding(graph);

        return GraphToRenderableGraphMapper.Map(
            graph,
            _transformer,
            xRange,
            yRange,
            screenSize);
    }
}