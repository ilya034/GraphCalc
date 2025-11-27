using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Entities;

namespace GraphCalc.Api.Mappers;

public class GraphToResponseMapper
{
    public static GraphResponse Map(Graph graph)
    {
        return new GraphResponse
        {
            Id = graph.Id,
            Expression = graph.Expression.Text,
            IndependentVariable = graph.IndependentVariable,
            Points = graph.Points.Select(p => new MathPointDto { X = p.X, Y = p.Y }).ToList(),
            Range = graph.Range != null ? new NumericRangeDto
            {
                Min = graph.Range.Min,
                Max = graph.Range.Max,
                Step = graph.Range.Step
            } : null
        };
    }
}
