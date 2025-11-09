namespace GraphCalc.Domain.ValueObjects;

public sealed record NumericRange
{
    public double Min { get; }
    public double Max { get; }
    public double Step { get; }

    private NumericRange(double min, double max, double step)
    {
        Min = min;
        Max = max;
        Step = step;
    }

    public static NumericRange Create(double min, double max, double? step = null)
    {
        if (double.IsNaN(min))
            throw new ArgumentException("Min must be a valid number");

        if (double.IsNaN(max))
            throw new ArgumentException("Max must be a valid number");

        if (min >= max)
            throw new ArgumentException("Min must be less than Max");

        var calculatedStep = step ?? (max - min) / 1000;

        return new NumericRange(min, max, calculatedStep);
    }

    public IEnumerable<double> GetValues()
    {
        for (double value = Min; value <= Max; value += Step)
            yield return value;
    }

    public int PointCount => (int)Math.Ceiling((Max - Min) / Step) + 1;

    public NumericRange WithMin(double min) => Create(min, Max, Step);
    public NumericRange WithMax(double max) => Create(Min, max, Step);
    public NumericRange WithStep(double step) => Create(Min, Max, step);
}