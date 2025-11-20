using System.Drawing;
using GraphCalc.Infrastructure.Facade;
using GraphCalc.Presentation.Facades;
using GraphCalc.Presentation.Models;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== GraphCalc: Separated Facades Demo ===\n");

        // Initialize facades
        var calculationFacade = new GraphCalculationFacade();
        var displayFacade = new GraphDisplayFacade();
        var screenSize = new Size(800, 600);

        // Demo 1: sin(x) with explicit Y range
        Console.WriteLine("Demo 1: sin(x) with explicit Y range");
        var sineGraph = calculationFacade.GetGraph("sin(x)", -Math.PI, Math.PI, 0.1);
        var sineRenderable = displayFacade.Display(sineGraph, -1.5, 1.5, screenSize);
        DisplayRenderableGraph(sineRenderable, "Sine Wave");

        // Demo 2: x*x with explicit Y range
        Console.WriteLine("\nDemo 2: x*x with explicit Y range");
        var quadraticGraph = calculationFacade.GetGraph("x*x", -5, 5, 0.1);
        var quadraticRenderable = displayFacade.Display(quadraticGraph, -1, 26, screenSize);
        DisplayRenderableGraph(quadraticRenderable, "Quadratic");

        // Demo 3: sin(x)^2 with auto Y-range
        Console.WriteLine("\nDemo 3: sin(x)^2 with auto Y-range calculation");
        var powerSineGraph = calculationFacade.GetGraphWithAutoYRange("pow(sin(x),2)", -Math.PI, Math.PI, 0.1);
        var powerSineRenderable = displayFacade.DisplayWithAutoYRange(powerSineGraph, screenSize);
        DisplayRenderableGraph(powerSineRenderable, "Power Sine (auto Y-range)");

        // Demo 4: Complex expression
        Console.WriteLine("\nDemo 4: Complex expression (x/2 + sin(x))");
        var complexGraph = calculationFacade.GetGraphWithAutoYRange("x/2 + sin(x)", -2*Math.PI, 2*Math.PI, 0.1);
        var complexRenderable = displayFacade.DisplayWithAutoYRange(complexGraph, screenSize);
        DisplayRenderableGraph(complexRenderable, "Complex Expression");
    }

    private static void DisplayRenderableGraph(RenderableGraph graph, string title)
    {
        Console.WriteLine($"  {title}:");
        Console.WriteLine($"    Screen points: {graph.ScreenPoints.Count}");
        Console.WriteLine($"    First 3 points: {string.Join(", ", graph.ScreenPoints.Take(3).Select(p => $"({p.X:F0}, {p.Y:F0})"))}");
        Console.WriteLine($"    Stroke: {graph.StrokeColor} (width: {graph.StrokeWidth})");
    }
}