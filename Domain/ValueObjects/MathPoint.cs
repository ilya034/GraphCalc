namespace GraphCalc.Domain.ValueObjects;

public readonly record struct MathPoint(double X, double Y)
{
    public System.Drawing.PointF ToPointF() => new((float)X, (float)Y);

    public System.Drawing.Point ToPoint() => new((int)Math.Round(X), (int)Math.Round(Y));

    public override string ToString() => $"({X:F2}, {Y:F2})";
}
