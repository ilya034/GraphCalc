using GraphCalc.Api.Dtos;
using GraphCalc.App;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.ValueObjects;

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

    public IEnumerable<Graph> GetAllGraphs()
    {
        var graphs = graphRepository.GetAll();
        return graphs;
    }

    public Graph GetGraphById(Guid id)
    {
        var graph = graphRepository.GetById(id);
        return graph;
    }

    public Graph CreateGraph(GraphCreateRequest request)
    {
        var graph = request.ToDomain();
        graphRepository.Add(graph);
        return graph;
    }

    public Graph CreateGraphWithAuthor(GraphCalculationRequest request, Guid authorId)
    {
        var graph = request.ToDomain(authorId);
        graphRepository.Add(graph);
        return graph;
    }

    public Graph UpdateGraph(Guid id, GraphDto graphDto)
    {
        var existingGraph = graphRepository.GetById(id);
        var updatedGraph = Graph.CreateWithId(
            id,
            graphDto.Range.ToDomain(),
            graphDto.AuthorId,
            graphDto.Items.ToDomain().ToList());

        graphRepository.Update(updatedGraph);
        return updatedGraph;
    }

    public void DeleteGraph(Guid id)
    {
        graphRepository.Delete(id);
    }

    public List<Series> CalculateGraph(Guid id)
    {
        var graph = graphRepository.GetById(id);
        var response = graphService.Calculate(graph);
        return response;
    }

    public List<Series> CalculateGraph(GraphCalculationRequest request)
    {
        var graph = request.ToDomain(Guid.Empty);
        var response = graphService.Calculate(graph);
        return response;
    }
}
