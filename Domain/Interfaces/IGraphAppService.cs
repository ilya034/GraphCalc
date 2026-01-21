using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Services;

public interface IGraphService
{
    IEnumerable<Graph> GetAllGraphs();
    Graph GetGraphById(Guid id);
    Graph CreateGraph(Graph graph);
    Graph UpdateGraph(Guid id, Graph graph);
    void DeleteGraph(Guid id);
    List<Series> CalculateGraph(Guid id);
    List<Series> CalculateGraph(Graph graph);
}
