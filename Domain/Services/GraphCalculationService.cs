using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.ValueObjects;
using GraphCalc.Infrastructure.ExpressionEvaluation;
using GraphCalc.Infrastructure.GraphCalculation;
using GraphCalc.Infrastructure.Repositories;
using GraphCalc.Api.Dtos;

namespace GraphCalc.Domain.Services;

public class GraphCalculationService : IGraphCalculationService
{
    private readonly IExpressionEvaluator _evaluator;
    private readonly IGraphRepository _graphRepository;
    private readonly IUserRepository _userRepository;
    private readonly InMemoryPublishedGraphRepository _publishedGraphRepository;
    private readonly InMemoryGraphSetRepository _graphSetRepository;

    public GraphCalculationService(
        IExpressionEvaluator? evaluator,
        IGraphRepository graphRepository,
        IUserRepository userRepository,
        InMemoryPublishedGraphRepository publishedGraphRepository,
        InMemoryGraphSetRepository graphSetRepository)
    {
        _evaluator = evaluator ?? new CodingSebExpressionEvaluator();
        _graphRepository = graphRepository;
        _userRepository = userRepository;
        _publishedGraphRepository = publishedGraphRepository;
        _graphSetRepository = graphSetRepository;
    }

    public Graph CalculateGraph(string expression, NumericRange xRange)
    {
        ValidateGraphCalculationRequest(expression, xRange);
        return GetGraphInternal(expression, xRange);
    }

    public Graph CalculateGraphWithAutoYRange(string expression, NumericRange xRange)
    {
        ValidateGraphCalculationRequest(expression, xRange);
        return GetGraphWithAutoYRangeInternal(expression, xRange);
    }

    public Graph CalculateAndSaveGraph(string expression, NumericRange xRange, bool autoYRange)
    {
        ValidateGraphCalculationRequest(expression, xRange);
        
        var graph = autoYRange
            ? GetGraphWithAutoYRangeInternal(expression, xRange)
            : GetGraphInternal(expression, xRange);

        _graphRepository.Add(graph);
        return graph;
    }

    public Graph SaveGraph(string expression, NumericRange xRange, bool autoYRange, string title, string? description, Guid userId)
    {
        ValidateGraphCalculationRequest(expression, xRange);
        
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var user = _userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        var graph = autoYRange
            ? GetGraphWithAutoYRangeInternal(expression, xRange)
            : GetGraphInternal(expression, xRange);

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
            if (graphRequest.XRange == null)
                throw new ArgumentException("XRange is required for all graphs");

            ValidateGraphCalculationRequest(
                graphRequest.Expression,
                graphRequest.XRange);

            var graph = graphRequest.AutoYRange
                ? GetGraphWithAutoYRangeInternal(
                    graphRequest.Expression,
                    graphRequest.XRange)
                : GetGraphInternal(
                    graphRequest.Expression,
                    graphRequest.XRange);

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

    public void ValidateGraphCalculationRequest(string expression, NumericRange xRange)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ArgumentException("Expression cannot be empty", nameof(expression));

        // NumericRange уже выполняет валидацию в методе Create
        // Проверки xMin >= xMax и xStep <= 0 уже выполняются в NumericRange.Create
    }

    private Graph GetGraphInternal(string expression, NumericRange xRange)
    {
        var mathExpr = MathExpression.Create(expression);
        var graph = Graph.Create(mathExpr, "x");
        graph.WithRange(xRange);

        var calculator = new NumericalGraphCalculator(_evaluator);
        var mathPoints = calculator.Calculate(graph).ToList();
        graph.SetPoints(mathPoints);

        return graph;
    }

    private Graph GetGraphWithAutoYRangeInternal(string expression, NumericRange xRange)
    {
        var mathExpr = MathExpression.Create(expression);
        var graph = Graph.Create(mathExpr, "x");
        graph.WithRange(xRange);

        var calculator = new NumericalGraphCalculator(_evaluator);
        var mathPoints = calculator.Calculate(graph).ToList();
        graph.SetPoints(mathPoints);

        var yValues = mathPoints.Where(p => !double.IsNaN(p.Y) && !double.IsInfinity(p.Y)).Select(p => p.Y);
        var yMin = yValues.Any() ? yValues.Min() : -1;
        var yMax = yValues.Any() ? yValues.Max() : 1;
        var padding = (yMax - yMin) * 0.1;
        
        var yRange = NumericRange.Create(yMin - padding, yMax + padding, 0.1);
        graph.WithRange(yRange);

        return graph;
    }
}