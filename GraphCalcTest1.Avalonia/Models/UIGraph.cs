using System;
using System.Collections.Generic;

namespace GraphCalcTest1.Avalonia.Models
{
    public class UIGraph
    {
        public Guid Id { get; set; }
        public string Expression { get; set; }
        public List<(double X, double Y)> Points { get; set; } = new();
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }
        public bool IsVisible { get; set; } = true;
        public System.Drawing.Color StrokeColor { get; set; } = System.Drawing.Color.Blue;
        public float StrokeWidth { get; set; } = 2f;
    }

    public class GraphRenderState
    {
        public double ViewMinX { get; set; } = -10;
        public double ViewMaxX { get; set; } = 10;
        public double ViewMinY { get; set; } = -10;
        public double ViewMaxY { get; set; } = 10;
        public double ZoomFactor { get; set; } = 1.0;
        public double PanX { get; set; } = 0;
        public double PanY { get; set; } = 0;
    }
}
