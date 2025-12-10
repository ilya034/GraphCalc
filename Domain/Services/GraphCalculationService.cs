using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Infrastructure.Facade;
using GraphCalc.Infrastructure.Repositories;
using GraphCalc.Api.Dtos;

namespace GraphCalc.Domain.Services;

public class GraphCalculationService : IGraphCalculationService
{
    private readonly GraphCalculationFacade _calculationFacade;
    private readonly IGraphRepository _graphRepository;
    private readonly IUserRepository _userRepository;
    private readonly InMemoryPublishedGraphRepository _publishedGraphRepository;
    private readonly InMemoryGraphSetRepository _graphSetRepository;

    public GraphCalculationService(
        GraphCalculationFacade calculationFacade,
        IGraphRepository graphRepository,
        IUserRepository userRepository,
        InMemoryPublishedGraphRepository publishedGraphRepository,
        InMemoryGraphSetRepository graphSetRepository)
    {
        _calculationFacade = calculationFacade;
        _graphRepository = graphRepository;
        _userRepository = userRepository;
        _publishedGraphRepository = publishedGraphRepository;
        _graphSetRepository = graphSetRepository;
    }

    public Graph CalculateGraph(string expression, double xMin, double xMax, double xStep)
    {
        ValidateGraphCalculationRequest(expression, xMin, xMax, xStep);
        return _calculationFacade.GetGraph(expression, xMin, xMax, xStep);
    }

    public Graph CalculateGraphWithAutoYRange(string expression, double xMin, double xMax, double xStep)
    {
        ValidateGraphCalculationRequest(expression, xMin, xMax, xStep);
        return _calculationFacade.GetGraphWithAutoYRange(expression, xMin, xMax, xStep);
    }

    public Graph CalculateAndSaveGraph(string expression, double xMin, double xMax, double xStep, bool autoYRange)
    {
        ValidateGraphCalculationRequest(expression, xMin, xMax, xStep);
        
        var graph = autoYRange
            ? _calculationFacade.GetGraphWithAutoYRange(expression, xMin, xMax, xStep)
            : _calculationFacade.GetGraph(expression, xMin, xMax, xStep);

        _graphRepository.Add(graph);
        return graph;
    }

    public Graph SaveGraph(string expression, double xMin, double xMax, double xStep, bool autoYRange, string title, string? description, Guid userId)
    {
        ValidateGraphCalculationRequest(expression, xMin, xMax, xStep);
        
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var user = _userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        var graph = autoYRange
            ? _calculationFacade.GetGraphWithAutoYRange(expression, xMin, xMax, xStep)
            : _calculationFacade.GetGraph(expression, xMin, xMax, xStep);

        _graphRepository.Add(graph);

        var publishedGraph = PublishedGraph.Create(
            userId,
            graph.Id,
            title,
            description);

        _publishedGraphRepository.Add(publishedGraph);
        user.PublishGraph(graph.Id);

        return graph;
    }

    public GraphSet SaveGraphSet(System.Collections.Generic.List<GraphCalc.Api.Dtos.SaveGraphRequest> graphs, string title, string? description, Guid userId)
    {
        if (graphs == null || graphs.Count == 0)
            throw new ArgumentException("GraphSet must contain at least one graph");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var user = _userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        var graphSet = GraphSet.Create();
        var graphDtos = new List<UserGraphDto>();

        foreach (var graphRequest in graphs)
        {
            ValidateGraphCalculationRequest(
                graphRequest.Expression,
                graphRequest.XMin,
                graphRequest.XMax,
                graphRequest.XStep);

            var graph = graphRequest.AutoYRange
                ? _calculationFacade.GetGraphWithAutoYRange(
                    graphRequest.Expression,
                    graphRequest.XMin,
                    graphRequest.XMax,
                    graphRequest.XStep)
                : _calculationFacade.GetGraph(
                    graphRequest.Expression,
                    graphRequest.XMin,
                    graphRequest.XMax,
                    graphRequest.XStep);

            _graphRepository.Add(graph);
            graphSet.AddGraph(graph);

            var publishedGraph = PublishedGraph.Create(
                userId,
                graph.Id,
                graphRequest.Title ?? $"Graph {graphDtos.Count + 1}",
                graphRequest.Description);

            _publishedGraphRepository.Add(publishedGraph);
            user.PublishGraph(graph.Id);

            graphDtos.Add(new UserGraphDto(
                Id: graph.Id,
                Expression: graph.Expression.Text,
                Title: graphRequest.Title ?? $"Graph {graphDtos.Count}",
                Description: graphRequest.Description
            ));
        }

        _graphSetRepository.Add(graphSet);
        return graphSet;
    }

    public void ValidateGraphCalculationRequest(string expression, double xMin, double xMax, double xStep)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ArgumentException("Expression cannot be empty", nameof(expression));

        if (xMin >= xMax)
            throw new ArgumentException("XMin must be less than XMax");

        if (xStep <= 0)
            throw new ArgumentException("XStep must be greater than 0");
    }
}