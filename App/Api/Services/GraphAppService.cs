using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Services;

public class GraphAppService
{
    public GraphCalculationResponse CalculateGraph(GraphCalculationRequest request)
    {
        // Create a new graph with the provided range and expression
        var range = request.Range ?? new NumericRange(-10, 10, 0.1);
        var graphItem = new GraphItem(request.Expression);
        
        // Create a graph (authorId is placeholder - should come from authentication)
        var graph = Graph.Create(range, Guid.NewGuid());
        graph.AddItem(graphItem);
        
        // Calculate points based on the expression and range
        // This is a simplified calculation - in a real app, you'd use a proper expression evaluator
        List<double> points = new List<double>();
        for (double x = range.Min; x <= range.Max; x += range.Step)
        {
            try
            {
                // Simple evaluation for demonstration
                // In a real app, use a library like NCalc, Math.NET, or similar
                double y = EvaluateSimpleExpression(request.Expression, x);
                points.Add(y);
            }
            catch
            {
                points.Add(double.NaN); // Add NaN for invalid points
            }
        }
        
        return new GraphCalculationResponse(
            points.ToArray(),
            range,
            request.Expression
        );
    }
    
    private double EvaluateSimpleExpression(string expression, double x)
    {
        // Very simple expression evaluation for demonstration
        // This should be replaced with a proper expression parser/evaluator
        if (string.IsNullOrWhiteSpace(expression))
            return 0;
        
        // Simple linear function: y = mx + b (where m and b are extracted from expression)
        // This is just a placeholder - real implementation would parse the expression properly
        if (expression.Contains("x"))
        {
            return x; // Simple y = x for demonstration
        }
        
        if (double.TryParse(expression, out double constant))
        {
            return constant;
        }
        
        return 0; // Default value
    }
}