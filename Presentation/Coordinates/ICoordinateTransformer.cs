using System.Drawing;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Presentation.Coordinates;

public interface ICoordinateTransformer
{
    PointF WorldToScreen(MathPoint worldPoint, NumericRange xRange, NumericRange yRange, Size screenSize);

    MathPoint ScreenToWorld(PointF screenPoint, NumericRange xRange, NumericRange yRange, Size screenSize);
}
