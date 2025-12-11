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
    private readonly IExpressionEvaluator evaluator;
    private readonly IGraphRepository graphRepository;
    private readonly IUserRepository userRepository;
    private readonly InMemoryPublishedGraphRepository publishedGraphRepository;
    private readonly InMemoryGraphSetRepository graphSetRepository;

    public GraphCalculationService(
        IExpressionEvaluator? evaluator,
        IGraphRepository graphRepository,
        IUserRepository userRepository,
        InMemoryPublishedGraphRepository publishedGraphRepository,
        InMemoryGraphSetRepository graphSetRepository)
    {
        this.evaluator = evaluator ?? new CodingSebExpressionEvaluator();
        this.graphRepository = graphRepository;
        this.userRepository = userRepository;
        this.publishedGraphRepository = publishedGraphRepository;
        this.graphSetRepository = graphSetRepository;
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

        graphRepository.Add(graph);
        return graph;
    }

    public Graph SaveGraph(string expression, NumericRange xRange, bool autoYRange, string title, string? description, Guid userId)
    {
        ValidateGraphCalculationRequest(expression, xRange);
        
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var user = userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        var graph = autoYRange
            ? GetGraphWithAutoYRangeInternal(expression, xRange)
            : GetGraphInternal(expression, xRange);

        graphRepository.Add(graph);

        var publishedGraph = PublishedGraph.Create(
            userId,
            graph.Id,
            title,
            description);

        publishedGraphRepository.Add(publishedGraph);
        user.PublishGraph(graph.Id);

        return graph;
    }

    public GraphSet SaveGraphSet(System.Collections.Generic.List<GraphCalc.Api.Dtos.SaveGraphRequest> graphs, string title, string? description, Guid userId)
    {
        if (graphs == null || graphs.Count == 0)
            throw new ArgumentException("GraphSet must contain at least one graph");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var user = userRepository.GetById(userId);
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

            graphRepository.Add(graph);
            graphSet.AddGraph(graph);

            var publishedGraph = PublishedGraph.Create(
                userId,
                graph.Id,
                graphRequest.Title ?? $"Graph {graphDtos.Count + 1}",
                graphRequest.Description);

            publishedGraphRepository.Add(publishedGraph);
            user.PublishGraph(graph.Id);

            graphDtos.Add(new UserGraphDto(
                Id: graph.Id,
                Expression: graph.Expression.Text,
                Title: graphRequest.Title ?? $"Graph {graphDtos.Count}",
                Description: graphRequest.Description
            ));
        }

        graphSetRepository.Add(graphSet);
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

        var calculator = new NumericalGraphCalculator(evaluator);
        var mathPoints = calculator.Calculate(graph).ToList();
        graph.SetPoints(mathPoints);

        return graph;
    }

    private Graph GetGraphWithAutoYRangeInternal(string expression, NumericRange xRange)
    {
        var mathExpr = MathExpression.Create(expression);
        var graph = Graph.Create(mathExpr, "x");
        graph.WithRange(xRange);

        var calculator = new NumericalGraphCalculator(evaluator);
        var mathPoints = calculator.Calculate(graph).ToList();
        graph.SetPoints(mathPoints);

        var yRange = CalculateYRangeFromGraphWithPadding(graph);
        graph.WithRange(yRange);

        return graph;
    }

    // Range calculation methods (integrated from GraphRangeService)
    public NumericRange CalculateYRangeFromGraph(Graph graph)
    {
        if (graph.Points == null || !graph.Points.Any())
            return NumericRange.Create(-1, 1, 0.1);

        var yValues = graph.Points
            .Where(p => !double.IsNaN(p.Y) && !double.IsInfinity(p.Y))
            .Select(p => p.Y);

        if (!yValues.Any())
            return NumericRange.Create(-1, 1, 0.1);

        var yMin = yValues.Min();
        var yMax = yValues.Max();

        return NumericRange.Create(yMin, yMax, 0.1);
    }

    public NumericRange CalculateYRangeFromGraphWithPadding(Graph graph, double paddingFactor = 0.1)
    {
        var yRange = CalculateYRangeFromGraph(graph);
        var padding = (yRange.Max - yRange.Min) * paddingFactor;

        return NumericRange.Create(
            yRange.Min - padding,
            yRange.Max + padding,
            yRange.Step);
    }
}