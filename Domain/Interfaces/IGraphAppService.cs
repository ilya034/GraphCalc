using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;
using GraphCalc.Api.Dtos;

namespace GraphCalc.Domain.Services;

public interface IGraphAppService
{
    IEnumerable<Graph> GetAllGraphs();
    Graph GetGraphById(Guid id);
    Graph CreateGraph(GraphCreateRequest request);
    Graph CreateGraphWithAuthor(GraphCalculationRequest request, Guid authorId);
    Graph UpdateGraph(Guid id, GraphDto graphDto);
    void DeleteGraph(Guid id);
    List<Series> CalculateGraph(Guid id);
    List<Series> CalculateGraph(GraphCalculationRequest request);
}
