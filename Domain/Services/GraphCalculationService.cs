using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.ValueObjects;
using GraphCalc.Infrastructure.ExpressionEvaluation;
using GraphCalc.Infrastructure.GraphCalculation;
using GraphCalc.Api.Dtos;

namespace GraphCalc.Domain.Services;

public class GraphCalculationService : IGraphCalculationService
{
    private readonly IExpressionEvaluator evaluator;
    private readonly IGraphRepository graphRepository;
    private readonly IUserRepository userRepository;
    private readonly IGraphSetRepository graphSetRepository;

    public GraphCalculationService(
        IExpressionEvaluator? evaluator,
        IGraphRepository graphRepository,
        IUserRepository userRepository,
        IGraphSetRepository graphSetRepository)
    {
        this.evaluator = evaluator ?? new CodingSebExpressionEvaluator();
        this.graphRepository = graphRepository;
        this.userRepository = userRepository;
        this.graphSetRepository = graphSetRepository;
    }

    public Graph CalculateGraph(string expression, NumericRange xRange)
    {
        ValidateGraphCalculationRequest(expression, xRange);
        return GetGraphInternal(expression, xRange, false);
    }

    public Graph CalculateGraphWithAutoYRange(string expression, NumericRange xRange)
    {
        ValidateGraphCalculationRequest(expression, xRange);
        return GetGraphInternal(expression, xRange, true);
    }

    public Graph CalculateAndSaveGraph(string expression, NumericRange xRange, bool autoYRange)
    {
        ValidateGraphCalculationRequest(expression, xRange);
        
        var graph = GetGraphInternal(expression, xRange, autoYRange);

        graphRepository.Add(graph);
        return graph;
    }

    public GraphSet SaveGraph(string expression, NumericRange xRange, bool autoYRange, string title, string? description, Guid userId)
    {
        ValidateGraphCalculationRequest(expression, xRange);
        
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var user = userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        var graph = GetGraphInternal(expression, xRange, autoYRange);

        graphRepository.Add(graph);

        var graphSet = GraphSet.Create(userId);
        graphSet.AddGraph(graph);
        graphSetRepository.Add(graphSet);

        return graphSet;
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

            var graph = GetGraphInternal(
                    graphRequest.Expression,
                    graphRequest.XRange,
                    graphRequest.AutoYRange);

            graphRepository.Add(graph);
            graphSet.AddGraph(graph);
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

    private Graph GetGraphInternal(string expression, NumericRange xRange, bool calculateYRange = false)
    {
        var mathExpr = MathExpression.Create(expression);
        var graph = Graph.Create(mathExpr, "x");
        graph.WithRange(xRange);

        if (calculateYRange)
        {
            var calculator = new NumericalGraphCalculator(evaluator);
            var mathPoints = calculator.Calculate(graph).ToList();
            var yRange = CalculateYRangeFromPoints(mathPoints);
            graph.WithRange(yRange);
        }

        return graph;
    }

    // Range calculation methods (integrated from GraphRangeService)
    private NumericRange CalculateYRangeFromPoints(IEnumerable<MathPoint> points)
    {
        if (points == null || !points.Any())
            return NumericRange.Create(-1, 1, 0.1);

        var yValues = points
            .Where(p => !double.IsNaN(p.Y) && !double.IsInfinity(p.Y))
            .Select(p => p.Y);

        if (!yValues.Any())
            return NumericRange.Create(-1, 1, 0.1);

        var yMin = yValues.Min();
        var yMax = yValues.Max();

        return NumericRange.Create(yMin, yMax, 0.1);
    }

    private NumericRange CalculateYRangeFromPointsWithPadding(IEnumerable<MathPoint> points, double paddingFactor = 0.1)
    {
        var yRange = CalculateYRangeFromPoints(points);
        var padding = (yRange.Max - yRange.Min) * paddingFactor;

        return NumericRange.Create(
            yRange.Min - padding,
            yRange.Max + padding,
            yRange.Step);
    }
}