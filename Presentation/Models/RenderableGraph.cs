using System.Drawing;

namespace GraphCalc.Presentation.Models;

public class RenderableGraph
{
    public Guid Id { get; init; }
    public string Label { get; init; } = string.Empty;
    public IReadOnlyList<PointF> ScreenPoints { get; init; } = Array.Empty<PointF>();
    public bool IsVisible { get; init; } = true;
    public Color StrokeColor { get; init; } = Color.Black;
    public float StrokeWidth { get; init; } = 1f;
    public LineStyle LineStyle { get; init; } = LineStyle.Solid;
    public bool ShowPoints { get; init; } = false;
    public float PointSize { get; init; } = 3f;
    public int ZIndex { get; init; } = 0;

    public override string ToString() => $"RenderableGraph({Label}, {ScreenPoints.Count} points)";
}
