using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using GraphCalcTest1.Avalonia.Models;

namespace GraphCalcTest1.Avalonia.Services
{
    public interface IGraphRenderingService
    {
        void RenderGraphs(
            DrawingContext ctx,
            IEnumerable<UIGraph> graphs,
            GraphRenderState renderState,
            Size canvasSize);
    }

    public class GraphRenderingService : IGraphRenderingService
    {
        private const float GridSpacing = 50f;
        private const float AxisThickness = 2f;
        private const float GridLineThickness = 0.5f;

        public void RenderGraphs(
            DrawingContext ctx,
            IEnumerable<UIGraph> graphs,
            GraphRenderState renderState,
            Size canvasSize)
        {
            if (canvasSize.Width <= 0 || canvasSize.Height <= 0)
                return;

            // Draw background
            ctx.FillRectangle(new SolidColorBrush(Colors.White), new Rect(canvasSize));

            // Draw grid
            DrawGrid(ctx, renderState, canvasSize);

            // Draw axes
            DrawAxes(ctx, renderState, canvasSize);

            // Draw graphs
            foreach (var graph in graphs.Where(g => g.IsVisible && g.Points.Count > 0))
            {
                DrawGraph(ctx, graph, renderState, canvasSize);
            }
        }

        private void DrawGrid(DrawingContext ctx, GraphRenderState renderState, Size canvasSize)
        {
            var gridPen = new Pen(new SolidColorBrush(Colors.LightGray), GridLineThickness);

            var xRange = renderState.ViewMaxX - renderState.ViewMinX;
            var yRange = renderState.ViewMaxY - renderState.ViewMinY;

            var xStep = CalculateGridStep(xRange);
            var yStep = CalculateGridStep(yRange);

            // Vertical lines
            var startX = Math.Ceiling(renderState.ViewMinX / xStep) * xStep;
            for (double x = startX; x <= renderState.ViewMaxX; x += xStep)
            {
                var screenX = WorldToScreenX(x, renderState, canvasSize);
                ctx.DrawLine(gridPen, new Point(screenX, 0), new Point(screenX, canvasSize.Height));
            }

            // Horizontal lines
            var startY = Math.Ceiling(renderState.ViewMinY / yStep) * yStep;
            for (double y = startY; y <= renderState.ViewMaxY; y += yStep)
            {
                var screenY = WorldToScreenY(y, renderState, canvasSize);
                ctx.DrawLine(gridPen, new Point(0, screenY), new Point(canvasSize.Width, screenY));
            }
        }

        private void DrawAxes(DrawingContext ctx, GraphRenderState renderState, Size canvasSize)
        {
            var axisPen = new Pen(new SolidColorBrush(Colors.Black), AxisThickness);

            // X axis
            if (renderState.ViewMinY <= 0 && renderState.ViewMaxY >= 0)
            {
                var screenY = WorldToScreenY(0, renderState, canvasSize);
                ctx.DrawLine(axisPen, new Point(0, screenY), new Point(canvasSize.Width, screenY));
            }

            // Y axis
            if (renderState.ViewMinX <= 0 && renderState.ViewMaxX >= 0)
            {
                var screenX = WorldToScreenX(0, renderState, canvasSize);
                ctx.DrawLine(axisPen, new Point(screenX, 0), new Point(screenX, canvasSize.Height));
            }
        }

        private void DrawGraph(DrawingContext ctx, UIGraph graph, GraphRenderState renderState, Size canvasSize)
        {
            if (graph.Points.Count < 2)
                return;

            var pen = new Pen(new SolidColorBrush(graph.StrokeColor.ToAvalonia()), graph.StrokeWidth);
            var screenPoints = graph.Points
                .Select(p => new Point(
                    WorldToScreenX(p.X, renderState, canvasSize),
                    WorldToScreenY(p.Y, renderState, canvasSize)))
                .ToList();

            for (int i = 0; i < screenPoints.Count - 1; i++)
            {
                ctx.DrawLine(pen, screenPoints[i], screenPoints[i + 1]);
            }
        }

        private double WorldToScreenX(double x, GraphRenderState renderState, Size canvasSize)
        {
            var normalized = (x - renderState.ViewMinX) / (renderState.ViewMaxX - renderState.ViewMinX);
            return normalized * canvasSize.Width;
        }

        private double WorldToScreenY(double y, GraphRenderState renderState, Size canvasSize)
        {
            var normalized = (y - renderState.ViewMinY) / (renderState.ViewMaxY - renderState.ViewMinY);
            return canvasSize.Height - (normalized * canvasSize.Height);
        }

        private double CalculateGridStep(double range)
        {
            var magnitude = Math.Pow(10, Math.Floor(Math.Log10(range)));
            var normalized = range / magnitude;

            if (normalized <= 2.5)
                return magnitude * 0.5;
            else if (normalized <= 5)
                return magnitude;
            else
                return magnitude * 2;
        }
    }

    public static class ColorExtensions
    {
        public static Color ToAvalonia(this System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
