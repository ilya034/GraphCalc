using System.Drawing;

namespace GraphCalc.Presentation.Models;

public class RenderableGraphCanvas
{
    public Guid Id { get; init; }
    public List<RenderableGraph> Graphs { get; init; } = new();
    public bool ShowLegend { get; init; } = true;
    public Size CanvasSize { get; set; }
}