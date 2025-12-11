using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Services;

public class GraphService : IGraphService
{
    private readonly IGraphSetRepository graphSetRepository;
    private readonly IUserRepository userRepository;
    private readonly IGraphCalculator calculator;

    public GraphService(
        IGraphSetRepository graphSetRepository,
        IUserRepository userRepository,
        IGraphCalculator calculator)
    {
        this.graphSetRepository = graphSetRepository;
        this.userRepository = userRepository;
        this.calculator = calculator;
    }

    public CalculatedGraphDto CalculatePreview(string expression, NumericRange range)
    {
        // Создаем временную сущность для калькулятора (не сохраняя в БД)
        var tempGraph = Graph.Create(expression);
        var points = calculator.Calculate(tempGraph, range).ToList();

        return new CalculatedGraphDto(
            Id: Guid.Empty,
            Expression: expression,
            Points: points
        );
    }

    public GraphSetDto CreateGraphSet(Guid userId, CreateGraphSetRequest request)
    {
        var user = userRepository.GetById(userId)
                   ?? throw new KeyNotFoundException("User not found");

        var graphs = request.Expressions.Select(expr => Graph.Create(expr)).ToList();

        var graphSet = GraphSet.Create(
            user.Id,
            request.Title,
            graphs,
            request.Description);

        graphSetRepository.Add(graphSet);

        return MapToDto(graphSet);
    }

    public GraphSetDto GetGraphSet(Guid id)
    {
        var set = graphSetRepository.GetById(id)
                  ?? throw new KeyNotFoundException("GraphSet not found");
        return MapToDto(set);
    }

    public IEnumerable<GraphSetDto> GetUserGraphSets(Guid userId)
    {
        var sets = graphSetRepository.GetByAuthorId(userId);
        return sets.Select(MapToDto);
    }

    public CalculatedGraphSetDto CalculateGraphSet(Guid graphSetId, NumericRange range)
    {
        var set = graphSetRepository.GetById(graphSetId)
                  ?? throw new KeyNotFoundException("GraphSet not found");

        var calculatedGraphs = new List<CalculatedGraphDto>();

        foreach (var graph in set.Graphs)
        {
            var points = calculator.Calculate(graph, range).ToList();
            calculatedGraphs.Add(new CalculatedGraphDto(graph.Id, graph.Expression, points));
        }

        return new CalculatedGraphSetDto(
            Id: set.Id,
            Title: set.Title,
            Range: range,
            Graphs: calculatedGraphs
        );
    }

    private GraphSetDto MapToDto(GraphSet set)
    {
        return new GraphSetDto(
            Id: set.Id,
            AuthorId: set.AuthorId,
            Expressions: set.Graphs.Select(g => new GraphDefinitionDto(g.Id, g.Expression)).ToList()
        );
    }
}