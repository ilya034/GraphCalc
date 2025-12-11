using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Infrastructure.GraphCalculation;

public sealed class NumericalGraphCalculator : IGraphCalculator
{
    private const int MaxIterations = 64;
    private readonly IExpressionEvaluator evaluator;

    public NumericalGraphCalculator(IExpressionEvaluator evaluator)
    {
        this.evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
    }

    public IEnumerable<MathPoint> Calculate(Graph graph)
    {
        var samples = SampleGraph(graph);

        foreach (var sample in samples)
        {
            if (!sample.isValid)
                continue;

            yield return new MathPoint(sample.x, sample.y);
        }
    }

    public IEnumerable<MathPoint> FindRoots(Graph graph, double precision = 0.001)
    {
        var samples = SampleGraph(graph).Where(p => p.isValid).ToList();

        if (samples.Count < 2)
            yield break;

        for (int i = 0; i < samples.Count - 1; i++)
        {
            var current = samples[i];
            var next = samples[i + 1];

            if (Math.Abs(current.y) <= precision)
            {
                yield return new MathPoint(current.x, 0);
                continue;
            }

            if (current.y * next.y > 0)
                continue;

            var root = RefineRoot(graph, current.x, next.x, precision);

            if (!double.IsNaN(root))
            {
                yield return new MathPoint(root, 0);
            }
        }
    }

    public IEnumerable<MathPoint> FindExtrema(Graph graph, double precision = 0.001)
    {
        var samples = SampleGraph(graph).Where(p => p.isValid).ToList();

        if (samples.Count < 3)
            yield break;

        for (int i = 1; i < samples.Count - 1; i++)
        {
            var prev = samples[i - 1];
            var current = samples[i];
            var next = samples[i + 1];

            double slopeLeft = (current.y - prev.y) / (current.x - prev.x);
            double slopeRight = (next.y - current.y) / (next.x - current.x);

            if (double.IsNaN(slopeLeft) || double.IsNaN(slopeRight))
                continue;

            if (Math.Abs(slopeLeft) <= precision && Math.Abs(slopeRight) <= precision)
            {
                yield return new MathPoint(current.x, current.y);
                continue;
            }

            if (slopeLeft < 0 && slopeRight > 0 ||
                slopeLeft > 0 && slopeRight < 0)
            {
                var extremumX = RefineExtremum(graph, prev.x, current.x, next.x, precision);

                if (double.IsNaN(extremumX))
                    extremumX = current.x;

                var extremumY = EvaluateSafe(graph, extremumX);

                if (double.IsNaN(extremumY))
                    continue;

                yield return new MathPoint(extremumX, extremumY);
            }
        }
    }

    private List<(double x, double y, bool isValid)> SampleGraph(Graph graph)
    {
        if (graph == null)
            throw new ArgumentNullException(nameof(graph));

        if (graph.Range == null)
            throw new ArgumentException("Graph range must be specified.", nameof(graph));

        var samples = new List<(double x, double y, bool isValid)>();

        foreach (var x in graph.Range.GetValues())
        {
            var y = EvaluateSafe(graph, x);
            samples.Add((x, y, !double.IsNaN(y) && !double.IsInfinity(y)));
        }

        return samples;
    }

    private double RefineRoot(Graph graph, double left, double right, double precision)
    {
        double fLeft = EvaluateSafe(graph, left);
        double fRight = EvaluateSafe(graph, right);

        if (double.IsNaN(fLeft) || double.IsNaN(fRight) || Math.Sign(fLeft) == Math.Sign(fRight))
            return double.NaN;

        for (int i = 0; i < MaxIterations; i++)
        {
            double mid = (left + right) / 2;
            double fMid = EvaluateSafe(graph, mid);

            if (double.IsNaN(fMid))
                return double.NaN;

            if (Math.Abs(fMid) <= precision || Math.Abs(right - left) / 2 <= precision)
                return mid;

            if (Math.Sign(fLeft) == Math.Sign(fMid))
            {
                left = mid;
                fLeft = fMid;
            }
            else
            {
                right = mid;
                fRight = fMid;
            }
        }

        return (left + right) / 2;
    }

    private double RefineExtremum(Graph graph, double left, double middle, double right, double precision)
    {
        double EvaluateDerivative(double x)
        {
            double step = Math.Max(precision, Math.Abs(right - left) / 100);
            double forward = EvaluateSafe(graph, x + step);
            double backward = EvaluateSafe(graph, x - step);

            if (double.IsNaN(forward) || double.IsNaN(backward))
                return double.NaN;

            return (forward - backward) / (2 * step);
        }

        double dLeft = EvaluateDerivative(left);
        double dMiddle = EvaluateDerivative(middle);
        double dRight = EvaluateDerivative(right);

        if (double.IsNaN(dLeft) || double.IsNaN(dRight))
            return double.NaN;

        double a = left;
        double b = right;
        double da = dLeft;
        double db = dRight;

        if (Math.Sign(da) == Math.Sign(db))
            return double.NaN;

        for (int i = 0; i < MaxIterations; i++)
        {
            double mid = (a + b) / 2;
            double dMid = EvaluateDerivative(mid);

            if (double.IsNaN(dMid))
                return double.NaN;

            if (Math.Abs(dMid) <= precision || Math.Abs(b - a) / 2 <= precision)
                return mid;

            if (Math.Sign(da) == Math.Sign(dMid))
            {
                a = mid;
                da = dMid;
            }
            else
            {
                b = mid;
                db = dMid;
            }
        }

        return (a + b) / 2;
    }

    private double EvaluateSafe(Graph graph, double x)
    {
        try
        {
            return evaluator.Evaluate(graph.Expression, x);
        }
        catch
        {
            return double.NaN;
        }
    }
}
