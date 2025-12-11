using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Services;

public interface IGraphRangeService
{
    NumericRange CalculateYRangeFromGraph(Graph graph);
    NumericRange CalculateYRangeFromGraphWithPadding(Graph graph, double paddingFactor = 0.1);
}