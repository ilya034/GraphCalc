using GraphCalc.Domain.Entities;

namespace GraphCalc.Domain.Services;

public interface IGraphCalculationService
{
    Graph CalculateGraph(string expression, double xMin, double xMax, double xStep);
    Graph CalculateGraphWithAutoYRange(string expression, double xMin, double xMax, double xStep);
    Graph CalculateAndSaveGraph(string expression, double xMin, double xMax, double xStep, bool autoYRange);
    Graph SaveGraph(string expression, double xMin, double xMax, double xStep, bool autoYRange, string title, string? description, Guid userId);
    GraphSet SaveGraphSet(System.Collections.Generic.List<GraphCalc.Api.Dtos.SaveGraphRequest> graphs, string title, string? description, Guid userId);
    void ValidateGraphCalculationRequest(string expression, double xMin, double xMax, double xStep);
}