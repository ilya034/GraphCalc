using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Infrastructure.ExpressionEvaluation;
using GraphCalc.Infrastructure.GraphCalculation;

namespace GraphCalc.Infrastructure.Facade;

public class GraphCalculationFacade
{
    private readonly IExpressionEvaluator evaluator;

    public GraphCalculationFacade(IExpressionEvaluator? evaluator = null)
    {
        this.evaluator = evaluator ?? new CodingSebExpressionEvaluator();
    }

    public Graph GetGraph(
        string expression,
        double xMin,
        double xMax,
        double xStep)
    {
        var mathExpr = MathExpression.Create(expression);
        var graph = Graph.Create(mathExpr, "x");
        var xRange = NumericRange.Create(xMin, xMax, xStep);
        graph.WithRange(xRange);

        var calculator = new NumericalGraphCalculator(evaluator);
        var mathPoints = calculator.Calculate(graph).ToList();
        graph.SetPoints(mathPoints);

        return graph;
    }

    public Graph GetGraphWithAutoYRange(
        string expression,
        double xMin,
        double xMax,
        double xStep)
    {
        var mathExpr = MathExpression.Create(expression);
        var graph = Graph.Create(mathExpr, "x");
        var xRange = NumericRange.Create(xMin, xMax, xStep);
        graph.WithRange(xRange);

        var calculator = new NumericalGraphCalculator(evaluator);
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
