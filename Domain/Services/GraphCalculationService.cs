using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Domain.Services;

public class GraphCalculationService
{
    private readonly IGraphCalculator graphCalculator;

    public GraphCalculationService(IGraphCalculator graphCalculator)
    {
        this.graphCalculator = graphCalculator ?? throw new ArgumentNullException(nameof(graphCalculator));
    }

    public List<Series> Calculate(Graph graph)
    {
        var results = new List<Series>();

        foreach (var item in graph.Items)
        {
            if (!item.IsVisible)
                results.Add(new Series(item.Expression, Array.Empty<MathPoint>()));

            var points = graphCalculator.Calculate(item.Expression, graph.Range).ToList();
            results.Add(new Series(item.Expression, points));
        }

        return results;
    }
}