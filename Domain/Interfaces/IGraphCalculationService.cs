using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Services;

public interface IGraphCalculationService
{
    List<Series> Calculate(Graph graph);
}