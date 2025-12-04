using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraphCalcTest1.Avalonia.Models;
using GraphCalcTest1.Avalonia.Services;

namespace GraphCalcTest1.Avalonia.ViewModels
{
    public partial class GraphDisplayViewModel : ObservableObject
    {
        private readonly IGraphApiService _graphApiService;
        private readonly IGraphRenderingService _renderingService;

        [ObservableProperty]
        private ObservableCollection<UIGraph> graphs = new();

        [ObservableProperty]
        private GraphRenderState renderState = new();

        [ObservableProperty]
        private string expression = "x^2";

        [ObservableProperty]
        private double xMin = -10;

        [ObservableProperty]
        private double xMax = 10;

        [ObservableProperty]
        private double xStep = 0.1;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string statusMessage = "Ready";

        private Avalonia.Size _canvasSize = new(800, 600);
        public Avalonia.Size CanvasSize
        {
            get => _canvasSize;
            set
            {
                SetProperty(ref _canvasSize, value);
                OnCanvasSizeChanged();
            }
        }

        public GraphDisplayViewModel()
        {
            _graphApiService = new GraphApiService();
            _renderingService = new GraphRenderingService();

            // Initialize render state
            RenderState = new GraphRenderState
            {
                ViewMinX = XMin,
                ViewMaxX = XMax,
                ViewMinY = -10,
                ViewMaxY = 10
            };
        }

        [RelayCommand]
        public async Task CalculateGraph()
        {
            if (string.IsNullOrWhiteSpace(Expression))
            {
                StatusMessage = "Expression is required";
                return;
            }

            IsLoading = true;
            StatusMessage = "Calculating...";

            try
            {
                var result = await _graphApiService.CalculateGraphAsync(
                    Expression,
                    XMin,
                    XMax,
                    XStep
                );

                var uiGraph = new UIGraph
                {
                    Id = result.Id,
                    Expression = result.Expression,
                    Points = result.Points
                        .Select(p => (p.X, p.Y))
                        .ToList(),
                    MinX = result.Range?.Min ?? XMin,
                    MaxX = result.Range?.Max ?? XMax,
                    MinY = result.Points.Count > 0 ? result.Points.Min(p => p.Y) : -10,
                    MaxY = result.Points.Count > 0 ? result.Points.Max(p => p.Y) : 10,
                    StrokeColor = GetColorForGraph(Graphs.Count)
                };

                Graphs.Add(uiGraph);

                // Update render state to fit all graphs
                UpdateRenderStateToFitGraphs();

                StatusMessage = $"Graph calculated ({result.Points.Count} points)";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void ZoomIn()
        {
            var centerX = (RenderState.ViewMinX + RenderState.ViewMaxX) / 2;
            var centerY = (RenderState.ViewMinY + RenderState.ViewMaxY) / 2;

            var xRange = (RenderState.ViewMaxX - RenderState.ViewMinX) / 4;
            var yRange = (RenderState.ViewMaxY - RenderState.ViewMinY) / 4;

            RenderState.ViewMinX = centerX - xRange;
            RenderState.ViewMaxX = centerX + xRange;
            RenderState.ViewMinY = centerY - yRange;
            RenderState.ViewMaxY = centerY + yRange;
            RenderState.ZoomFactor *= 2;

            OnPropertyChanged(nameof(RenderState));
        }

        [RelayCommand]
        public void ZoomOut()
        {
            var centerX = (RenderState.ViewMinX + RenderState.ViewMaxX) / 2;
            var centerY = (RenderState.ViewMinY + RenderState.ViewMaxY) / 2;

            var xRange = (RenderState.ViewMaxX - RenderState.ViewMinX) / 2;
            var yRange = (RenderState.ViewMaxY - RenderState.ViewMinY) / 2;

            RenderState.ViewMinX = centerX - xRange;
            RenderState.ViewMaxX = centerX + xRange;
            RenderState.ViewMinY = centerY - yRange;
            RenderState.ViewMaxY = centerY + yRange;
            RenderState.ZoomFactor /= 2;

            OnPropertyChanged(nameof(RenderState));
        }

        [RelayCommand]
        public void ResetView()
        {
            RenderState = new GraphRenderState
            {
                ViewMinX = XMin,
                ViewMaxX = XMax,
                ViewMinY = -10,
                ViewMaxY = 10,
                ZoomFactor = 1.0
            };

            UpdateRenderStateToFitGraphs();
        }

        [RelayCommand]
        public void ClearGraphs()
        {
            Graphs.Clear();
            ResetView();
            StatusMessage = "Graphs cleared";
        }

        [RelayCommand]
        public void RemoveGraph(UIGraph graph)
        {
            Graphs.Remove(graph);
            if (Graphs.Count > 0)
            {
                UpdateRenderStateToFitGraphs();
            }
            else
            {
                ResetView();
            }
        }

        private void UpdateRenderStateToFitGraphs()
        {
            if (Graphs.Count == 0)
            {
                ResetView();
                return;
            }

            var allMinX = Graphs.Min(g => g.MinX);
            var allMaxX = Graphs.Max(g => g.MaxX);
            var allMinY = Graphs.Min(g => g.MinY);
            var allMaxY = Graphs.Max(g => g.MaxY);

            var xPadding = (allMaxX - allMinX) * 0.1;
            var yPadding = (allMaxY - allMinY) * 0.1;

            RenderState.ViewMinX = allMinX - xPadding;
            RenderState.ViewMaxX = allMaxX + xPadding;
            RenderState.ViewMinY = allMinY - yPadding;
            RenderState.ViewMaxY = allMaxY + yPadding;

            OnPropertyChanged(nameof(RenderState));
        }

        private void OnCanvasSizeChanged()
        {
            OnPropertyChanged(nameof(CanvasSize));
        }

        private System.Drawing.Color GetColorForGraph(int index)
        {
            var colors = new[]
            {
                System.Drawing.Color.Blue,
                System.Drawing.Color.Red,
                System.Drawing.Color.Green,
                System.Drawing.Color.Purple,
                System.Drawing.Color.Orange,
                System.Drawing.Color.Brown,
                System.Drawing.Color.Pink,
                System.Drawing.Color.Cyan
            };

            return colors[index % colors.Length];
        }
    }
}
