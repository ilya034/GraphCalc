using System.Drawing;
using GraphCalc.Domain.Entities;
using GraphCalc.Presentation.Models;

namespace GraphCalc.Presentation.Services;

public interface IGraphDisplayService
{
    RenderableGraph DisplayGraph(Graph graph, double yMin, double yMax, Size screenSize);
    RenderableGraph DisplayGraphWithAutoYRange(Graph graph, Size screenSize);
}