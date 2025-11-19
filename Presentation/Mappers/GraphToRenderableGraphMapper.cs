using System.Drawing;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;
using GraphCalc.Presentation.Coordinates;
using GraphCalc.Presentation.Models;

namespace GraphCalc.Presentation.Mappers;

public static class GraphToRenderableGraphMapper
{
    public static RenderableGraph Map(
        Graph graph,
        ICoordinateTransformer transformer,
        NumericRange xRange,
        NumericRange yRange,
        Size screenSize)
    {
        var screenPoints = graph.Points
            .Select(p => transformer.WorldToScreen(p, xRange, yRange, screenSize))
            .ToArray();

        var style = GraphStyle.Default;

        return new RenderableGraph
        {
            Id = graph.Id,
            Label = graph.IndependentVariable,
            ScreenPoints = screenPoints,
            IsVisible = true,
            StrokeColor = style.Color,
            StrokeWidth = (float)style.LineThickness,
            LineStyle = style.LineStyle,
            ShowPoints = style.ShowPoints,
            PointSize = (float)style.PointSize
        };
    }

    public static IEnumerable<RenderableGraph> MapMany(
        IEnumerable<Graph> graphs,
        ICoordinateTransformer transformer,
        NumericRange xRange,
        NumericRange yRange,
        Size screenSize)
        => graphs.Select(g => Map(g, transformer, xRange, yRange, screenSize));
}
