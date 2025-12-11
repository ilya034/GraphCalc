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
    private readonly IGraphSetRepository graphSetRepository;
    private readonly IUserRepository userRepository;

    public GraphCalculationService(
        IExpressionEvaluator? evaluator,
        IGraphSetRepository graphSetRepository,
        IUserRepository userRepository)
    {
        this.evaluator = evaluator ?? new CodingSebExpressionEvaluator();
        this.graphSetRepository = graphSetRepository;
        this.userRepository = userRepository;
    }

    public UserGraphSetDto CalculateGraphSet(Guid graphSetId)
    {
        var graphSet = graphSetRepository.GetById(graphSetId);
        if (graphSet == null)
            throw new KeyNotFoundException($"GraphSet with ID {graphSetId} not found");

        var calculator = new NumericalGraphCalculator(evaluator);
        var itemDtos = new List<GraphItemDto>();

        foreach (var item in graphSet.Items)
        {
            var range = item.Range ?? graphSet.GlobalRange;
            if (range == null)
                throw new InvalidOperationException("Range is not defined for graph item");

            var mathPoints = calculator.Calculate(Graph.Create(
                MathExpression.Create(item.Expression.Text), "x").WithRange(range)).ToList();

            itemDtos.Add(new GraphItemDto(
                Id: item.Id,
                Expression: item.Expression.Text,
                IndependentVariable: item.Expression.VariableName,
                IsVisible: item.IsVisible,
                Range: range,
                Points: mathPoints
            ));
        }

        return new UserGraphSetDto(
            Id: graphSet.Id,
            Title: "Calculated Graph Set",
            GlobalRange: graphSet.GlobalRange,
            Items: itemDtos
        );
    }

    public UserGraphSetDto CreateAndCalculateGraphSet(
        List<SaveGraphRequest> graphRequests,
        string title,
        string? description,
        Guid userId,
        NumericRange? globalRange = null)
    {
        if (graphRequests == null || graphRequests.Count == 0)
            throw new ArgumentException("GraphSet must contain at least one graph");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var user = userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        var graphSet = GraphSet.Create(userId);
        if (globalRange != null)
            graphSet.WithGlobalRange(globalRange);

        var calculator = new NumericalGraphCalculator(evaluator);
        var itemDtos = new List<GraphItemDto>();

        foreach (var graphRequest in graphRequests)
        {
            if (graphRequest.XRange == null)
                throw new ArgumentException("XRange is required for all graphs");

            var range = graphRequest.XRange;
            var mathPoints = calculator.Calculate(Graph.Create(
                MathExpression.Create(graphRequest.Expression), "x").WithRange(range)).ToList();

            graphSet.AddGraph(graphRequest.Expression, "x");

            itemDtos.Add(new GraphItemDto(
                Id: graphSet.Items.Last().Id,
                Expression: graphRequest.Expression,
                IndependentVariable: "x",
                IsVisible: true,
                Range: range,
                Points: mathPoints
            ));
        }

        graphSetRepository.Add(graphSet);

        return new UserGraphSetDto(
            Id: graphSet.Id,
            Title: title,
            Description: description,
            GlobalRange: globalRange,
            Items: itemDtos
        );
    }

    public UserGraphSetDto AddGraphToSet(
        Guid graphSetId,
        SaveGraphRequest graphRequest)
    {
        var graphSet = graphSetRepository.GetById(graphSetId);
        if (graphSet == null)
            throw new KeyNotFoundException($"GraphSet with ID {graphSetId} not found");

        if (graphRequest.XRange == null)
            throw new ArgumentException("XRange is required");

        var calculator = new NumericalGraphCalculator(evaluator);
        var range = graphRequest.XRange;
        var mathPoints = calculator.Calculate(Graph.Create(
            MathExpression.Create(graphRequest.Expression), "x").WithRange(range)).ToList();

        graphSet.AddGraph(graphRequest.Expression, "x");

        var newItem = graphSet.Items.Last();
        var itemDto = new GraphItemDto(
            Id: newItem.Id,
            Expression: graphRequest.Expression,
            IndependentVariable: "x",
            IsVisible: true,
            Range: range,
            Points: mathPoints
        );

        graphSetRepository.Update(graphSet);

        return new UserGraphSetDto(
            Id: graphSet.Id,
            Title: "Updated Graph Set",
            GlobalRange: graphSet.GlobalRange,
            Items: graphSet.Items.Select(item => new GraphItemDto(
                Id: item.Id,
                Expression: item.Expression.Text,
                IndependentVariable: item.Expression.VariableName,
                IsVisible: item.IsVisible,
                Range: item.Range,
                Points: null
            )).ToList()
        );
    }

    public UserGraphSetDto UpdateGraphInSet(
        Guid graphSetId,
        Guid graphItemId,
        string newExpression)
    {
        var graphSet = graphSetRepository.GetById(graphSetId);
        if (graphSet == null)
            throw new KeyNotFoundException($"GraphSet with ID {graphSetId} not found");

        var item = graphSet.Items.FirstOrDefault(i => i.Id == graphItemId);
        if (item == null)
            throw new KeyNotFoundException($"GraphItem with ID {graphItemId} not found");

        var range = item.Range ?? graphSet.GlobalRange;
        if (range == null)
            throw new InvalidOperationException("Range is not defined for graph item");

        graphSet.UpdateGraphExpression(graphItemId, newExpression);

        var calculator = new NumericalGraphCalculator(evaluator);
        var mathPoints = calculator.Calculate(Graph.Create(
            MathExpression.Create(newExpression), "x").WithRange(range)).ToList();

        var itemDtos = graphSet.Items.Select(i => {
            var currentRange = i.Range ?? graphSet.GlobalRange;
            var currentPoints = i.Id == graphItemId ? mathPoints : null;

            return new GraphItemDto(
                Id: i.Id,
                Expression: i.Expression.Text,
                IndependentVariable: i.Expression.VariableName,
                IsVisible: i.IsVisible,
                Range: currentRange,
                Points: currentPoints
            );
        }).ToList();

        graphSetRepository.Update(graphSet);

        return new UserGraphSetDto(
            Id: graphSet.Id,
            Title: "Updated Graph Set",
            GlobalRange: graphSet.GlobalRange,
            Items: itemDtos
        );
    }
}