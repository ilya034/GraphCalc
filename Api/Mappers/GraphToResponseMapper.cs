using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Mappers;

public class GraphToResponseMapper
{
    public static GraphResponse Map(Graph graph)
    {
        return new GraphResponse(
            Id: graph.Id,
            Expression: graph.Expression.Text,
            IndependentVariable: graph.IndependentVariable,
            Points: graph.Points.ToList(),
            Range: graph.Range
        );
    }
}
