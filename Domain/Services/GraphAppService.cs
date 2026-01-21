using GraphCalc.Api.Dtos;
using GraphCalc.App;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Domain.Services;

internal class GraphAppService : IGraphAppService
{
    private readonly IGraphRepository graphRepository;
    private readonly GraphCalculationService graphService;

    public GraphAppService(IGraphRepository graphRepository, GraphCalculationService graphService)
    {
        this.graphRepository = graphRepository;
        this.graphService = graphService;
    }

    public IEnumerable<GraphDto> GetAllGraphs()
    {
        var graphs = graphRepository.GetAll();
        return graphs.ToDto();
    }

    public GraphDto GetGraphById(Guid id)
    {
        var graph = graphRepository.GetById(id);
        return graph.ToDto();
    }

    public GraphDto CreateGraph(GraphCreateRequest request)
    {
        var graph = request.ToDomain();
        graphRepository.Add(graph);
        return graph.ToDto();
    }

    public GraphDto CreateGraphWithAuthor(GraphCalculationRequest request, Guid authorId)
    {
        var graph = request.ToDomain(authorId);
        graphRepository.Add(graph);
        return graph.ToDto();
    }

    public GraphDto UpdateGraph(Guid id, GraphDto graphDto)
    {
        var existingGraph = graphRepository.GetById(id);
        var updatedGraph = Graph.CreateWithId(
            id,
            graphDto.Range.ToDomain(),
            graphDto.AuthorId,
            graphDto.Items.ToDomain().ToList());

        graphRepository.Update(updatedGraph);
        return updatedGraph.ToDto();
    }

    public void DeleteGraph(Guid id)
    {
        graphRepository.Delete(id);
    }

    public GraphCalculationResponse CalculateGraph(Guid id)
    {
        var graph = graphRepository.GetById(id);
        var response = graphService.Calculate(graph);
        return new GraphCalculationResponse(response.ToDto().ToList());
    }

    public GraphCalculationResponse CalculateGraph(GraphCalculationRequest request)
    {
        var graph = request.ToDomain(Guid.Empty);
        var response = graphService.Calculate(graph);
        return new GraphCalculationResponse(response.ToDto().ToList());
    }
}
