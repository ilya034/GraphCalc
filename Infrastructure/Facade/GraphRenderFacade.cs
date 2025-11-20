namespace GraphCalc.Infrastructure.Facade;

public class GraphRenderFacade
{
    private readonly IExpressionEvaluator evaluator;
    private readonly ICoordinateTransformer transformer;

    public GraphRenderFacade(
        IExpressionEvaluator? evaluator = null,
        ICoordinateTransformer? transformer = null)
    {
        this.evaluator = evaluator ?? new CodingSebExpressionEvaluator();
        this.transformer = transformer ?? new SimpleCoordinateTransformer();
    }

    /// <summary>
    /// Get a renderable graph for the given mathematical expression
    /// </summary>
    /// <param name="expression">Mathematical expression (e.g., "sin(x)", "x*x")</param>
    /// <param name="xMin">Minimum X value</param>
    /// <param name="xMax">Maximum X value</param>
    /// <param name="xStep">Step size for X axis</param>
    /// <param name="yMin">Minimum Y value for display</param>
    /// <param name="yMax">Maximum Y value for display</param>
    /// <param name="screenSize">Screen size for rendering</param>
    /// <returns>Renderable graph ready for display</returns>
    public RenderableGraph GetGraph(
        string expression,
        double xMin,
        double xMax,
        double xStep,
        double yMin,
        double yMax,
        Size screenSize)
    {
        // Create domain model
        var mathExpr = MathExpression.Create(expression);
        var graph = Graph.Create(mathExpr, "x");
        var xRange = NumericRange.Create(xMin, xMax, xStep);
        graph.WithRange(xRange);

        // Calculate math points
        var calculator = new NumericalGraphCalculator(evaluator);
        var mathPoints = calculator.Calculate(graph).ToList();
        graph.SetPoints(mathPoints);

        // Transform to presentation model
        var yRange = NumericRange.Create(yMin, yMax, 0.1);
        var renderableGraph = GraphToRenderableGraphMapper.Map(
            graph,
            transformer,
            xRange,
            yRange,
            screenSize);

        return renderableGraph;
    }

    /// <summary>
    /// Get a renderable graph with auto-calculated Y range
    /// </summary>
    public RenderableGraph GetGraphWithAutoYRange(
        string expression,
        double xMin,
        double xMax,
        double xStep,
        Size screenSize)
    {
        var mathExpr = MathExpression.Create(expression);
        var graph = Graph.Create(mathExpr, "x");
        var xRange = NumericRange.Create(xMin, xMax, xStep);
        graph.WithRange(xRange);

        var calculator = new NumericalGraphCalculator(evaluator);
        var mathPoints = calculator.Calculate(graph).ToList();
        graph.SetPoints(mathPoints);

        // Calculate Y range from points
        var yValues = mathPoints.Where(p => !double.IsNaN(p.Y) && !double.IsInfinity(p.Y)).Select(p => p.Y);
        var yMin = yValues.Any() ? yValues.Min() : -1;
        var yMax = yValues.Any() ? yValues.Max() : 1;
        var padding = (yMax - yMin) * 0.1;
        
        var yRange = NumericRange.Create(yMin - padding, yMax + padding, 0.1);
        var renderableGraph = GraphToRenderableGraphMapper.Map(
            graph,
            transformer,
            xRange,
            yRange,
            screenSize);

        return renderableGraph;
    }
}
