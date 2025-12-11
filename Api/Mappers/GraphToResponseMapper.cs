using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Entities;

namespace GraphCalc.Api.Mappers;

public class GraphToResponseMapper
{
    public static GraphResponse Map(Graph graph)
    {
        return new GraphResponse(
            Id: graph.Id,
            Expression: graph.Expression.Text,
            IndependentVariable: graph.IndependentVariable,
            Points: new List<MathPoint>(), // Points are not stored in the repository anymore
            Range: graph.Range
        );
    }
}
