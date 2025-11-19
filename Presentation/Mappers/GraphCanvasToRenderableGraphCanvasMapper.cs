using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;
using GraphCalc.Presentation.Coordinates;
using GraphCalc.Presentation.Models;
using System.Drawing;

namespace GraphCalc.Presentation.Mappers;

public static class GraphSetToRenderableGraphSetMapper
{
    public static RenderableGraphCanvas Map(
        GraphSet graphCanvas,
        ICoordinateTransformer transformer,
        NumericRange xRange,
        NumericRange yRange,
        Size screenSize)
    {
        var renderableGraphs = GraphToRenderableGraphMapper.MapMany(
            graphCanvas.Graphs,
            transformer,
            xRange,
            yRange,
            screenSize);

        return new RenderableGraphCanvas
        {
            Id = graphCanvas.Id,
            Graphs = renderableGraphs.ToList(),
            CanvasSize = screenSize
        };
    }
}