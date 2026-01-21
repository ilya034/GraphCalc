using GraphCalc.Api.Dtos;

namespace GraphCalc.Domain.Services;

public interface IGraphAppService
{
    IEnumerable<GraphDto> GetAllGraphs();
    GraphDto GetGraphById(Guid id);
    GraphDto CreateGraph(GraphCreateRequest request);
    GraphDto CreateGraphWithAuthor(GraphCalculationRequest request, Guid authorId);
    GraphDto UpdateGraph(Guid id, GraphDto graphDto);
    void DeleteGraph(Guid id);
    GraphCalculationResponse CalculateGraph(Guid id);
    GraphCalculationResponse CalculateGraph(GraphCalculationRequest request);
}
