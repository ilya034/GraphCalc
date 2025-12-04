using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using GraphCalcTest1.Avalonia.Models;
using GraphCalcTest1.Avalonia.Services;
using GraphCalcTest1.Avalonia.ViewModels;

namespace GraphCalcTest1.Avalonia.Controls
{
    public class GraphCanvasControl : Control
    {
        private readonly IGraphRenderingService _renderingService;
        private GraphDisplayViewModel _viewModel;
        private Point _lastMousePoint;
        private bool _isPanning = false;

        public static readonly StyledProperty<GraphDisplayViewModel> ViewModelProperty =
            AvaloniaProperty.Register<GraphCanvasControl, GraphDisplayViewModel>(nameof(ViewModel));

        public GraphDisplayViewModel ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public GraphCanvasControl()
        {
            _renderingService = new GraphRenderingService();
            
            ClipToBounds = true;
            Background = new SolidColorBrush(Colors.White);

            PointerMoved += OnPointerMoved;
            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerWheelChanged += OnPointerWheelChanged;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _viewModel = ViewModel ?? (DataContext as GraphDisplayViewModel);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (_viewModel == null)
                return;

            _viewModel.CanvasSize = new Size(Bounds.Width, Bounds.Height);

            _renderingService.RenderGraphs(
                context,
                _viewModel.Graphs,
                _viewModel.RenderState,
                new Size(Bounds.Width, Bounds.Height)
            );
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is GraphDisplayViewModel vm)
            {
                _viewModel = vm;
                vm.PropertyChanged += (s, e) => InvalidateVisual();
            }
        }

        private void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                _isPanning = true;
                _lastMousePoint = e.GetPosition(this);
            }
        }

        private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            _isPanning = false;
        }

        private void OnPointerMoved(object sender, PointerEventArgs e)
        {
            if (!_isPanning || _viewModel == null)
                return;

            var currentPoint = e.GetPosition(this);
            var deltaX = currentPoint.X - _lastMousePoint.X;
            var deltaY = currentPoint.Y - _lastMousePoint.Y;

            var xRange = _viewModel.RenderState.ViewMaxX - _viewModel.RenderState.ViewMinX;
            var yRange = _viewModel.RenderState.ViewMaxY - _viewModel.RenderState.ViewMinY;

            var xDelta = -(deltaX / Bounds.Width) * xRange;
            var yDelta = (deltaY / Bounds.Height) * yRange;

            _viewModel.RenderState.ViewMinX += xDelta;
            _viewModel.RenderState.ViewMaxX += xDelta;
            _viewModel.RenderState.ViewMinY += yDelta;
            _viewModel.RenderState.ViewMaxY += yDelta;

            _lastMousePoint = currentPoint;
            InvalidateVisual();
        }

        private void OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            if (_viewModel == null)
                return;

            var delta = e.Delta.Y;
            if (delta > 0)
            {
                _viewModel.ZoomInCommand.Execute(null);
            }
            else if (delta < 0)
            {
                _viewModel.ZoomOutCommand.Execute(null);
            }

            InvalidateVisual();
        }
    }
}
