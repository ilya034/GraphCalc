using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Services;

public interface IGraphCalculationService
{
    Graph CalculateGraph(string expression, NumericRange xRange);
    Graph CalculateGraphWithAutoYRange(string expression, NumericRange xRange);
    Graph CalculateAndSaveGraph(string expression, NumericRange xRange, bool autoYRange);
    Graph SaveGraph(string expression, NumericRange xRange, bool autoYRange, string title, string? description, Guid userId);
    GraphSet SaveGraphSet(System.Collections.Generic.List<GraphCalc.Api.Dtos.SaveGraphRequest> graphs, string title, string? description, Guid userId);
    void ValidateGraphCalculationRequest(string expression, NumericRange xRange);

    // Range calculation methods
    NumericRange CalculateYRangeFromGraph(Graph graph);
    NumericRange CalculateYRangeFromGraphWithPadding(Graph graph, double paddingFactor = 0.1);
}