using GraphCalc.Api.Dtos;
using GraphCalc.App;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.ValueObjects;
using GraphCalc.Domain.Exceptions;

namespace GraphCalc.Domain.Services;

internal class GraphService : IGraphService
{
    private readonly IGraphRepository graphRepository;
    private readonly GraphCalculationService graphService;

    public GraphService(IGraphRepository graphRepository, GraphCalculationService graphService)
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
        if (id == Guid.Empty)
            throw new ArgumentException("Graph ID cannot be empty.", nameof(id));

        try
        {
            var graph = graphRepository.GetById(id);
            return graph;
        }
        catch (KeyNotFoundException)
        {
            throw new EntityNotFoundException(nameof(Graph), id);
        }
    }

    public Graph CreateGraph(Graph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);
        graphRepository.Add(graph);
        return graph;
    }

    public Graph UpdateGraph(Guid id, Graph graph)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Graph ID cannot be empty.", nameof(id));

        ArgumentNullException.ThrowIfNull(graph);

        // Проверяем, что граф существует
        try
        {
            graphRepository.GetById(id);
        }
        catch (KeyNotFoundException)
        {
            throw new EntityNotFoundException(nameof(Graph), id);
        }

        // ensure the domain model has the correct id
        var updatedGraph = Graph.CreateWithId(id, graph.Range, graph.AuthorId, graph.Items.ToList());
        graphRepository.Update(updatedGraph);
        return updatedGraph;
    }

    public void DeleteGraph(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Graph ID cannot be empty.", nameof(id));

        try
        {
            graphRepository.Delete(id);
        }
        catch (KeyNotFoundException)
        {
            throw new EntityNotFoundException(nameof(Graph), id);
        }
    }

    public List<Series> CalculateGraph(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Graph ID cannot be empty.", nameof(id));

        try
        {
            var graph = graphRepository.GetById(id);
            var response = graphService.Calculate(graph);
            return response;
        }
        catch (KeyNotFoundException)
        {
            throw new EntityNotFoundException(nameof(Graph), id);
        }
    }

    public List<Series> CalculateGraph(Graph graph)
    {
        ArgumentNullException.ThrowIfNull(graph);
        var response = graphService.Calculate(graph);
        return response;
    }
}
