using System.Drawing;

namespace GraphCalc.Presentation.Models;

public record GraphStyle
{
    public Color Color { get; init; }
    public double LineThickness { get; init; }
    public LineStyle LineStyle { get; init; }
    public bool ShowPoints { get; init; }
    public double PointSize { get; init; }

    private GraphStyle(
        Color color,
        double lineThickness = 2.0,
        LineStyle lineStyle = LineStyle.Solid,
        bool showPoints = false,
        double pointSize = 3.0)
    {
        Color = color;
        LineThickness = lineThickness;
        LineStyle = lineStyle;
        ShowPoints = showPoints;
        PointSize = pointSize;
    }

    public static GraphStyle Create(
        Color color,
        double lineThickness = 2.0,
        LineStyle lineStyle = LineStyle.Solid,
        bool showPoints = false,
        double pointSize = 3.0)
    {
        if (lineThickness <= 0 || lineThickness > 10)
            throw new ArgumentException("Line thickness must be between 0 and 10");

        if (pointSize <= 0 || pointSize > 20)
            throw new ArgumentException("Point size must be between 0 and 20");

        return new GraphStyle(color, lineThickness, lineStyle, showPoints, pointSize);
    }

    public GraphStyle WithColor(Color color) 
        => this with { Color = color };

    public GraphStyle WithThickness(double thickness) 
        => Create(Color, thickness, LineStyle, ShowPoints, PointSize);

    public GraphStyle WithLineStyle(LineStyle style) 
        => this with { LineStyle = style };

    public GraphStyle WithPoints(bool show, double size = 3.0) 
        => Create(Color, LineThickness, LineStyle, show, size);

    public static readonly GraphStyle Default = new(
        Color.Blue,
        lineThickness: 2.0,
        lineStyle: LineStyle.Solid,
        showPoints: false,
        pointSize: 3.0);
}