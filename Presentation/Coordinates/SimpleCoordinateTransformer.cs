using System.Drawing;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Presentation.Coordinates;

public class SimpleCoordinateTransformer : ICoordinateTransformer
{
    public PointF WorldToScreen(MathPoint worldPoint, NumericRange xRange, NumericRange yRange, Size screenSize)
    {
        if (xRange == null) throw new ArgumentNullException(nameof(xRange));
        if (yRange == null) throw new ArgumentNullException(nameof(yRange));
        if (screenSize.Width <= 0 || screenSize.Height <= 0)
            throw new ArgumentException("Screen size must be positive", nameof(screenSize));

        var xSpan = xRange.Max - xRange.Min;
        var ySpan = yRange.Max - yRange.Min;

        if (xSpan == 0 || ySpan == 0)
            return new PointF(screenSize.Width / 2f, screenSize.Height / 2f);

        var normalizedX = (worldPoint.X - xRange.Min) / xSpan;
        var normalizedY = (yRange.Max - worldPoint.Y) / ySpan; // Y is inverted

        var screenX = (float)(normalizedX * screenSize.Width);
        var screenY = (float)(normalizedY * screenSize.Height);

        return new PointF(screenX, screenY);
    }

    public MathPoint ScreenToWorld(PointF screenPoint, NumericRange xRange, NumericRange yRange, Size screenSize)
    {
        if (xRange == null) throw new ArgumentNullException(nameof(xRange));
        if (yRange == null) throw new ArgumentNullException(nameof(yRange));
        if (screenSize.Width <= 0 || screenSize.Height <= 0)
            throw new ArgumentException("Screen size must be positive", nameof(screenSize));

        var xSpan = xRange.Max - xRange.Min;
        var ySpan = yRange.Max - yRange.Min;

        if (xSpan == 0 || ySpan == 0)
            return new MathPoint((xRange.Min + xRange.Max) / 2, (yRange.Min + yRange.Max) / 2);

        var normalizedX = screenPoint.X / screenSize.Width;
        var normalizedY = screenPoint.Y / screenSize.Height;

        var worldX = xRange.Min + normalizedX * xSpan;
        var worldY = yRange.Max - normalizedY * ySpan; // Y is inverted

        return new MathPoint(worldX, worldY);
    }
}
